using DerMistkaefer.DvbLive.TriasCommunication.Configuration;
using Knapcode.TorSharp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DerMistkaefer.DvbLive.TriasCommunication.HostedServices
{
    /// <summary>
    /// Hosted Service to manage the TorSharpProxy.
    /// Download Dependencies, Start, Stop, GetNewIdentity
    /// </summary>
    internal class TorSharpProxyHostedService : IHostedService, IDisposable
    {
        private readonly IOptions<TriasConfiguration> _triasConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TorSharpProxyHostedService> _logger;
        private readonly ITorSharpProxy _proxy;
        private Timer? _timer;

        public TorSharpProxyHostedService(
            IOptions<TriasConfiguration> triasConfiguration,
            IHttpClientFactory httpClientFactory,
            ILogger<TorSharpProxyHostedService> logger
            )
        {
            _triasConfiguration = triasConfiguration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _proxy = new TorSharpProxy(triasConfiguration.Value.TorSharpSettings);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            await new TorSharpToolFetcher(_triasConfiguration.Value.TorSharpSettings, httpClient).FetchAsync().ConfigureAwait(false);

            await _proxy.ConfigureAndStartAsync().ConfigureAwait(false);

            _timer = new Timer(SwitchToNewIdentity, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        private void SwitchToNewIdentity(object state)
        {
            _proxy.GetNewIdentityAsync().Wait();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _proxy.Stop();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _proxy.Dispose();
        }
    }
}
