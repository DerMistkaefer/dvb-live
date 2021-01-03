using Newtonsoft.Json;

namespace DerMistkaefer.DvbLive.IPGeolocation.Models
{
    internal class IpStackLanguageResponse
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; } = "";

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = "";

        [JsonProperty(PropertyName = "native")]
        public string Native { get; set; } = "";
    }
}
