using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DerMistkaefer.DvbLive.GetPublicTransportLines.Data;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;

namespace DerMistkaefer.DvbLive.GetPublicTransportLines.Germany
{
    internal class DresdenPublicTransportLinesCollector : IPublicTransportLinesCollector
    {
        private readonly Uri _tramDataUri = new Uri("https://kommisdd.dresden.de/net4/public/ogcapi/collections/L457/items");
        private readonly Uri _busDataUri = new Uri("https://kommisdd.dresden.de/net4/public/ogcapi/collections/L1076/items");
        
        private readonly HttpClient _httpClient;

        public DresdenPublicTransportLinesCollector(IHttpClientFactory httpClientFactory)
        {
            _httpClient = new HttpClient();
        }

        /// <inheritdoc cref="IPublicTransportLinesCollector"/>
        public async Task<IEnumerable<PublicTransportLine>> GetPublicTransportLines()
        {
            var tasks = new[] {GetTramData(), GetBusData()};
            var response = await Task.WhenAll(tasks).ConfigureAwait(false);
            
            return response.SelectMany(x => x);
        }
        
        private async Task<IEnumerable<PublicTransportLine>> GetTramData()
        {
            var response = await BaseHttpRequest(_tramDataUri).ConfigureAwait(false);
            return MapData(response.Features, (output, data) =>
            {

            });
        }

        private async Task<IEnumerable<PublicTransportLine>> GetBusData()
        {
            var response = await BaseHttpRequest(_busDataUri).ConfigureAwait(false);
            return MapData(response.Features, (output, data) =>
            {

            });
        }

        private async Task<DresdenResponse> BaseHttpRequest(Uri uri)
        {
            var response = await _httpClient.GetAsync(uri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            // GeoJson have problem with crs value in MultiLineString so we remove it.
            var regex = new Regex(@"""crs"":{.*?}},");
            var serializableString = regex.Replace(responseString, string.Empty);
            
            return JsonConvert.DeserializeObject<DresdenResponse>(serializableString);
        }

        private IEnumerable<PublicTransportLine> MapData(IEnumerable<Feature> lines, Action<PublicTransportLine, IDictionary<string, object>> actionMapAdditionalData)
        {
            var output = new List<PublicTransportLine>();
            foreach (var line in lines)
            {
                var data = line.Properties;
                var title = data.ContainsKey("text") ? $"{data["text"]}" : "";
                var from = data.ContainsKey("anfang") ? $"{data["anfang"]}" : "";
                var to = data.ContainsKey("ende") ? $"{data["ende"]}" : "";
                var urlLineChange = data.ContainsKey("url_linienaenderung") ? data["url_linienaenderung"]?.ToString() : null;
                var clockingTime = data.ContainsKey("takt") ? $"{data["takt"]}" : "";
                var clockingInformation = data.ContainsKey("takt_bemerkung") ? $"{data["takt_bemerkung"]}" : "";
                var clocking = !string.IsNullOrWhiteSpace(clockingTime) || !string.IsNullOrWhiteSpace(clockingInformation) ? $"{clockingTime} - {clockingInformation}" : null;
                line.Properties.Clear();
                var newOutput = new PublicTransportLine(title, from, to, line)
                {
                    UrlLineChange = urlLineChange,
                    Clocking = clocking
                };
                actionMapAdditionalData(newOutput, data);
                output.Add(newOutput);
            }
            return output;
        }

        private class DresdenResponse
        {
            public IEnumerable<Feature> Features { get; set; } = null!;
        }
    }
}