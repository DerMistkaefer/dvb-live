using DerMistkaefer.DvbLive.TriasCommunication.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using vdo.trias;

namespace DerMistkaefer.DvbLive.TriasCommunication
{
    /// <summary>
    /// HttpClient for the Trias-Api.
    /// </summary>
    internal class TriasHttpClient : ITriasHttpClient, IDisposable
    {
        public int ApiRequestsCount { get; private set; }
        public long DownloadedBytes { get; private set; }

        private readonly HttpClient _httpClient;

        public TriasHttpClient(IHttpClientFactory httpClientFactory, IOptions<TriasConfiguration> triasConfiguration)
        {
            _httpClient = httpClientFactory.CreateClient(TriasConfiguration.HttpClientFactoryClientName);
            _httpClient.BaseAddress = triasConfiguration.Value.TriasUrl;
        }

        /// <inheritdoc cref="ITriasHttpClient"/>
        public async Task<TType> BaseTriasCall<TType>(object requestPayload)
        {
            ApiRequestsCount++;
            var serviceRequest = new ServiceRequestStructure1
            {
                RequestTimestamp = System.DateTime.Now,
                RequestorRef = new ParticipantRefStructure() { Value = "OpenService" },
                RequestPayload = new RequestPayloadStructure() { Item = requestPayload }
            };
            var trias = new Trias { Item = serviceRequest };
            var text = XmlSerialisation(trias);

            using var content = new StringContent(text, Encoding.UTF8, "text/xml");
            var response = await _httpClient.PostAsync("", content).ConfigureAwait(false);
            DownloadedBytes += response.Content?.Headers?.ContentLength ?? 0;
            await EnsureSuccessTriasResponse(response).ConfigureAwait(false);

            await using var responseStream = await response.Content!.ReadAsStreamAsync().ConfigureAwait(false);
            var responseTrias = XmlDeserialisation<Trias>(responseStream);
            var serviceDelievery = (ServiceDeliveryStructure1)responseTrias.Item;
            DeliveryPayloadStructure delevieryPayload = serviceDelievery.DeliveryPayload;

            return (TType)delevieryPayload.Item;
        }

        private static async Task EnsureSuccessTriasResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var requestString = "";
                if (response.RequestMessage.Content != null)
                {
                    requestString = await response.RequestMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                var responeString = "";
                if (response.Content != null)
                {
                    responeString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                var ex = new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}).");
                ex.Data["Request"] = requestString;
                ex.Data["Response"] = responeString;
                throw ex;
            }
        }

        private static string XmlSerialisation(object data)
        {
            var xmlSerializer = new XmlSerializer(data.GetType());
            using StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, data);

            return textWriter.ToString();
        }

        private static TType XmlDeserialisation<TType>(Stream data)
        {
            using var xmlReader = new XmlTextReader(data)
            {
                WhitespaceHandling = WhitespaceHandling.Significant,
                Normalization = true,
                XmlResolver = null
            };
            var xmlSerializer = new XmlSerializer(typeof(TType));

            return (TType)xmlSerializer.Deserialize(xmlReader);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
