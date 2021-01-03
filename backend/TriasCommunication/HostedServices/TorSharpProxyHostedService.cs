using DerMistkaefer.DvbLive.TriasCommunication.Configuration;
using Knapcode.TorSharp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DerMistkaefer.DvbLive.IPGeolocation;

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
        private readonly IIpGeolocation _ipGeolocation;
        private readonly ILogger<TorSharpProxyHostedService> _logger;
        private readonly ITorSharpProxy _proxy;
        private readonly HttpClient _proxyHttpClient;
        private Timer? _timer;

        public TorSharpProxyHostedService(
            IOptions<TriasConfiguration> triasConfiguration,
            IHttpClientFactory httpClientFactory,
            IIpGeolocation ipGeolocation,
            ILogger<TorSharpProxyHostedService> logger
            )
        {
            _triasConfiguration = triasConfiguration;
            _httpClientFactory = httpClientFactory;
            _ipGeolocation = ipGeolocation;
            _logger = logger;
            var config = triasConfiguration.Value;
            _proxy = new TorSharpProxy(config.TorSharpSettings);
            var proxyHttpClientHandler = new HttpClientHandler
            {
                Proxy = new WebProxy(new Uri($"http://localhost:{config.TorSharpSettings.PrivoxySettings.Port}"))
            };
            _proxyHttpClient = new HttpClient(proxyHttpClientHandler);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            await new TorSharpToolFetcher(_triasConfiguration.Value.TorSharpSettings, httpClient).FetchAsync().ConfigureAwait(false);

            await _proxy.ConfigureAndStartAsync().ConfigureAwait(false);
            await CheckIdentity().ConfigureAwait(false);

            _timer = new Timer(SwitchToNewIdentity, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        private void SwitchToNewIdentity(object state)
            => SwitchToNewIdentityAsync(state).Wait();
        
        private async Task SwitchToNewIdentityAsync(object state)
        {
            await _proxy.GetNewIdentityAsync().ConfigureAwait(false);
            await CheckIdentity().ConfigureAwait(false);
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

        private async Task CheckIdentity()
        {
            var ipResponse = await _ipGeolocation.GeolocateOwnAdress(_proxyHttpClient).ConfigureAwait(false);
            _logger.LogInformation($"New TriasCommincation IP - {ipResponse}");
        }
    }
}
