using System;
using System.Net.Http;

namespace DerMistkaefer.DvbLive.ProxyHttp.FreeProxySharp
{
	/// <summary>
	/// Works with random proxy HttpClient
	/// </summary>
	public class HttpProxyFactory : IHttpClientFactory
	{
		private readonly IHttpProxyConfiguration _config;
		private readonly IHttpClientFactory _http;

		/// <summary>
		/// Http Client Factory für 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="clientFactory"></param>
		public HttpProxyFactory(IHttpProxyConfiguration config, IHttpClientFactory clientFactory)
		{
			_config = config;
			_http = clientFactory;
		}
		
		/// <inheritdoc />
		public HttpClient CreateClient(string name)
		{
			return name == "TriasCommunication.HttpClient" 
				? GetClientProxy(name) 
				: _http.CreateClient(name);
		}

		/// <summary>
		/// Returns random proxy HttpClient by configuration 
		/// </summary>
		private HttpClient GetClientProxy(string name, int? num = null)
		{
			if (_config.Proxies?.Length > 0)
			{
				var number = num ?? new Random().Next(1, _config.Proxies.Length);

				return _http.CreateClient($"{name}.{number}");
			}
			else
			{
				return _http.CreateClient(name);
			}
		}
	}
}

