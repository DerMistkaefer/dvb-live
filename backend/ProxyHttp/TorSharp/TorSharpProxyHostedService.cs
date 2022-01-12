using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DerMistkaefer.DvbLive.IPGeolocation;
using Knapcode.TorSharp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DerMistkaefer.DvbLive.ProxyHttp.TorSharp
{
    /// <summary>
    /// Hosted Service to manage the TorSharpProxy.
    /// Download Dependencies, Start, Stop, GetNewIdentity
    /// </summary>
    internal class TorSharpProxyHostedService : IHostedService, IDisposable
    {
        private readonly IOptions<TorSharpSettings> _triasConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IIpGeolocation _ipGeolocation;
        private readonly ILogger<TorSharpProxyHostedService> _logger;
        private readonly ITorSharpProxy _proxy;
        private readonly HttpClient _proxyHttpClient;
        private Timer? _timer;

        public TorSharpProxyHostedService(
            IOptions<TorSharpSettings> torSharpConfiguration,
            IHttpClientFactory httpClientFactory,
            IIpGeolocation ipGeolocation,
            ILogger<TorSharpProxyHostedService> logger
            )
        {
            _triasConfiguration = torSharpConfiguration;
            _httpClientFactory = httpClientFactory;
            _ipGeolocation = ipGeolocation;
            _logger = logger;
            var config = torSharpConfiguration.Value;
            _proxy = new TorSharpProxy(config);
            var proxyHttpClientHandler = new HttpClientHandler
            {
                Proxy = new WebProxy(new Uri($"http://localhost:{config.PrivoxySettings.Port}"))
            };
            _proxyHttpClient = new HttpClient(proxyHttpClientHandler);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            await new TorSharpToolFetcher(_triasConfiguration.Value, httpClient).FetchAsync().ConfigureAwait(false);

            await _proxy.ConfigureAndStartAsync().ConfigureAwait(false);
            await CheckIdentity().ConfigureAwait(false);

            _timer = new Timer(SwitchToNewIdentity, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        private void SwitchToNewIdentity(object? state)
            => SwitchToNewIdentityAsync(state).Wait();
        
        private async Task SwitchToNewIdentityAsync(object? _)
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
            var ipResponse = await _ipGeolocation.GeolocateOwnAddress(_proxyHttpClient).ConfigureAwait(false);
            _logger.LogInformation("New ProxyHttp IP - {IPAddress}", ipResponse);
        }
    }
}
