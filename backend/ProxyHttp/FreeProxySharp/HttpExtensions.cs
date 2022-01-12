using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Serilog;

namespace DerMistkaefer.DvbLive.ProxyHttp.FreeProxySharp
{
	/// <summary>
	/// Extensions for configure common & proxy HttpClients
	/// </summary>
	public static class HttpExtensions
    {

	    /// <summary>
		/// HttpClient DI settings by name with proxy
		/// </summary>
		public static void AddHttpClientProxy(this IServiceCollection services, string name, IHttpProxyConfiguration config, Func<HttpResponseMessage, bool>? whenRetry = null)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			services.AddHttpClientProxy(name, config.Proxies, config.Retry, config.RetryFirstDelay, whenRetry);
		}
	    
		private static void AddHttpClientProxy(this IServiceCollection services, string name, IHttpProxyServer[] proxies,
			int retry, int retryFirstDelay,
			Func<HttpResponseMessage, bool>? whenRetry = null)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException(nameof(name));
			if (proxies == null || proxies.Length == 0)
				throw new ArgumentNullException(nameof(proxies));

			// check Result status code; OK -> continue
			whenRetry ??= res => res?.StatusCode != HttpStatusCode.OK;

			var x = 1;
			foreach (var p in proxies)
			{
				var proxyName = $"{name}.{x}";

				services.AddHttpClient(proxyName)
					// proxy
					// https://stackoverflow.com/questions/29856543/httpclient-and-using-proxy-constantly-getting-407
					.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
					{
						Proxy = new WebProxy(p.Ip, p.Port),
					})
					.AddTransientHttpErrorPolicy(builder => builder
						.OrResult(whenRetry)
						// exponential waiting; number of retry by parameters
						.WaitAndRetryAsync(retry,
							retryAttempt => GetDelay(retryFirstDelay, retryAttempt),
							onRetry: (outcome, timespan, retryAttempt, context) =>
							{
								Log.Warning($"Retry [client] delay: {timespan.TotalSeconds}s #{retryAttempt} url: '{outcome.Result?.RequestMessage?.RequestUri?.OriginalString}'");
							}));

				Log.Information("HttpClient {Name} proxy {Ip}:{Port} {Note}", proxyName, p.Ip, p.Port, p.Note);
				x++;
			}
		}

		/// <summary>
		/// exponential waiting
		/// </summary>
		private static TimeSpan GetDelay(int firstRetryDelay, int retryAttempt)
		{
			var jitterer = new Random();
			var waitFor = firstRetryDelay + (int)Math.Pow(2, retryAttempt);
			var spanWaitFor = TimeSpan.FromSeconds(waitFor) + TimeSpan.FromMilliseconds(jitterer.Next(0, (waitFor * 100)));
			return spanWaitFor;
		}
    }
}
