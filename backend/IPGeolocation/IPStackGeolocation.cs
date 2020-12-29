using DerMistkaefer.DvbLive.IPGeolocation.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DerMistkaefer.DvbLive.IPGeolocation
{
    internal class IpStackGeolocation : IIpGeolocation
    {
        private readonly string _accessKey;
        private HttpClient? _defaultHttpClient;

        private HttpClient DefaultHttpClient
        {
            get { return _defaultHttpClient ??= new HttpClient(); }
        }

        public IpStackGeolocation(string accessKey)
        {
            _accessKey = accessKey;
        }

        private async Task Test(IPAddress ipAddress)
        {
            var ipCheckUri = new Uri($"http://api.ipstack.com/{ipAddress}?access_key={_accessKey}&format=1");
            var response = await DefaultHttpClient.PostAsync(ipCheckUri, null).ConfigureAwait(false);
            var ipStackResponse = await response.Content.ReadAsAsync<IpStackResponse>().ConfigureAwait(false);
        }

        public string AccessKey { get; set; }
    }
}
