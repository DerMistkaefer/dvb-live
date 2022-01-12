using System;
using System.Net;
using System.Net.Http;
using Knapcode.TorSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DerMistkaefer.DvbLive.ProxyHttp.TorSharp
{
    public static class ProxyHttpTorSharpLoader
    {

        public static void AddProxyHttpTorSharp(this IServiceCollection services, string httpClientName)
        {
            services.AddHttpClient(httpClientName)
                .ConfigurePrimaryHttpMessageHandler(BuildProxyHttpMessageHandler);
            services.Configure<TorSharpSettings>((x) => GetTorSharpSettings(x, httpClientName));
            services.AddHostedService<TorSharpProxyHostedService>();
        }
        
        private static HttpClientHandler BuildProxyHttpMessageHandler(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IOptions<TorSharpSettings>>();
            if (config is null)
            {
                throw new NullReferenceException($"The Configuration '{nameof(TorSharpSettings)}' could not be found.");
            }

            return new HttpClientHandler
            {
                Proxy = new WebProxy(new Uri($"http://localhost:{config.Value.PrivoxySettings.Port}"))
            };
        }
        
        private static void GetTorSharpSettings(TorSharpSettings settings, string httpClientName)
        {
            settings.PrivoxySettings.MaxClientConnections = 20000;
            settings.TorSettings.ControlPassword = $"{httpClientName}{DateTime.Now}";
        }
    }
    
}