using DerMistkaefer.DvbLive.IPGeolocation.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DerMistkaefer.DvbLive.IPGeolocation.Configuration;
using Microsoft.Extensions.Options;

namespace DerMistkaefer.DvbLive.IPGeolocation
{
    /// <summary>
    /// IpGeolocation over the IpStack Api.
    /// https://ipstack.com/
    /// </summary>
    internal class IpStackGeolocation : IIpGeolocation
    {
        private readonly string _accessKey;
        private HttpClient? _defaultHttpClient;

        private HttpClient DefaultHttpClient
        {
            get { return _defaultHttpClient ??= new HttpClient(); }
        }

        public IpStackGeolocation(IOptions<IpGeolocationConfiguration> ipGeoLocationOptions)
        {
            var config = ipGeoLocationOptions.Value;
            _accessKey = config.AccessKey;
        }
        
        /// <inheritdoc cref="IIpGeolocation"/>
        public async Task<string> GeolocateOwnAdress(HttpClient? httpClient = null)
        {
            var checkClient = httpClient ?? DefaultHttpClient;
            var ipCheckUri = new Uri($"http://api.ipstack.com/check?access_key={_accessKey}&format=1");
            var response = await checkClient.PostAsync(ipCheckUri, null).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var ipStackResponse = await response.Content.ReadAsAsync<IpStackResponse>().ConfigureAwait(false);

            return $"{ipStackResponse.ContinentName} - {ipStackResponse.RegionName} - {ipStackResponse.City}";
        }

        /// <inheritdoc cref="IIpGeolocation"/>
        public async Task<string> GeolocateAdress(IPAddress ipAddress)
        {
            var ipCheckUri = new Uri($"http://api.ipstack.com/{ipAddress}?access_key={_accessKey}&format=1");
            var response = await DefaultHttpClient.PostAsync(ipCheckUri, null).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var ipStackResponse = await response.Content.ReadAsAsync<IpStackResponse>().ConfigureAwait(false);

            return $"{ipStackResponse.ContinentName} - {ipStackResponse.RegionName} - {ipStackResponse.City}";
        }
    }
}
