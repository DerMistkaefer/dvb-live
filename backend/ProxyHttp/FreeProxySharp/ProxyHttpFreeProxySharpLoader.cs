using System;
using DerMistkaefer.DvbLive.ProxyHttp.FreeProxySharp.FreeProxy;
using Microsoft.Extensions.DependencyInjection;

namespace DerMistkaefer.DvbLive.ProxyHttp.FreeProxySharp
{
    public static class ProxyHttpFreeProxySharpLoader
    {
        public static void AddProxyHttpFreeProxySharp(this IServiceCollection services)
        {
            var config = new ProxyConfig();
            config.CheckAndAssignToConfig();
            services.AddHttpClientProxy("TriasCommunication.HttpClient", config);
            services.AddSingleton<HttpProxyFactory>();
            //services.AddHostedService<FreeProxySharpHostedService>();
        }
        
        private class ProxyConfig : IHttpProxyConfiguration
        {
            public int Retry => 2;
            public int RetryFirstDelay => 1;

            public IHttpProxyServer[] Proxies { get; set; } = Array.Empty<IHttpProxyServer>();
        }
        
        private class FreeProxySharpHostedService
        {
        
        }
    }
}