using DerMistkaefer.DvbLive.TriasCommunication.Configuration;
using DerMistkaefer.DvbLive.TriasCommunication.Data;
using Microsoft.Extensions.Logging;
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
    internal sealed class TriasHttpClient : ITriasHttpClient, IDisposable
    {
        private readonly ILogger<TriasHttpClient> _logger;
        private readonly HttpClient _httpClient;

        /// <inheritdoc cref="ITriasHttpClient"/>
        public event TriasEventHandlers.RequestFinishedEventHandler? RequestFinished;

        public TriasHttpClient(IHttpClientFactory httpClientFactory, IOptions<TriasConfiguration> triasConfiguration, ILogger<TriasHttpClient> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient(TriasConfiguration.HttpClientFactoryClientName);
            _httpClient.BaseAddress = triasConfiguration.Value.TriasUrl;
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

        }

        /// <inheritdoc cref="ITriasHttpClient"/>
        public async Task<TType> BaseTriasCall<TType>(object requestPayload)
        {
            var serviceRequest = new ServiceRequestStructure1
            {
                RequestTimestamp = System.DateTime.Now,
                RequestorRef = new ParticipantRefStructure { Value = "OpenService" },
                RequestPayload = new RequestPayloadStructure { Item = requestPayload }
            };
            var trias = new Trias { Item = serviceRequest };
            var text = XmlSerialisation(trias);

            var response = await TriasRequestClientHandling(text).ConfigureAwait(false);
            OnRequestFinished(response);
            await EnsureSuccessTriasResponse(response).ConfigureAwait(false);

            await using var responseStream = await response.Content!.ReadAsStreamAsync().ConfigureAwait(false);
            var responseTrias = XmlDeserialisation<Trias>(responseStream);
            var serviceDelievery = (ServiceDeliveryStructure1)responseTrias.Item;
            DeliveryPayloadStructure delevieryPayload = serviceDelievery.DeliveryPayload;

            return (TType)delevieryPayload.Item;
        }

        private async Task<HttpResponseMessage> TriasRequestClientHandling(string requestXmlString, int retryCount = 0)
        {
            try
            {
                using var content = new StringContent(requestXmlString, Encoding.UTF8, "text/xml");
                return await _httpClient.PostAsync("", content).ConfigureAwait(false);
            }
            catch (TaskCanceledException) when (retryCount < 5)
            {
                _logger.LogInformation("Retry Request");
                retryCount++;
                return await TriasRequestClientHandling(requestXmlString, retryCount).ConfigureAwait(false);
            }
        }

        private static async Task EnsureSuccessTriasResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var requestString = await GetHttpContentAsStringSave(response.RequestMessage.Content).ConfigureAwait(false);
                var responeString = await GetHttpContentAsStringSave(response.Content).ConfigureAwait(false);
                
                var ex = new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}).");
                ex.Data["Request"] = requestString;
                ex.Data["Response"] = responeString;
                throw ex;
            }
        }

        private static async Task<string> GetHttpContentAsStringSave(HttpContent httpContent)
        {
            var httpContentString = "";
            try
            {
                httpContentString = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (ObjectDisposedException ex)
            {
                httpContentString = ex.Message;
            }

            return httpContentString;
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

        private void OnRequestFinished(HttpResponseMessage response)
        {
            var downloadedBytes = response.Content?.Headers?.ContentLength ?? 0;
            var eventArgs = new RequestFinishedEventArgs(downloadedBytes);

            var handler = RequestFinished;
            handler?.Invoke(this, eventArgs);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
