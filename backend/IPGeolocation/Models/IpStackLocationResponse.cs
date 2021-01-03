using System;
using Newtonsoft.Json;

namespace DerMistkaefer.DvbLive.IPGeolocation.Models
{
    internal class IpStackLocationResponse
    {
        [JsonProperty(PropertyName = "geoname_id")]
        public int? GeonameId { get; set; }

        [JsonProperty(PropertyName = "capital")]
        public string Capital { get; set; } = "";

        [JsonProperty(PropertyName = "languages")]
        public IpStackLanguageResponse[] Languages { get; set; } = Array.Empty<IpStackLanguageResponse>();

        [JsonProperty(PropertyName = "country_flag")]
        public string CountryFlag { get; set; } = "";

        [JsonProperty(PropertyName = "country_flag_emoji")]
        public string CountryFlagEmoji { get; set; } = "";

        [JsonProperty(PropertyName = "country_flag_emoji_unicode")]
        public string CountryFlagEmojiUnicode { get; set; } = "";

        [JsonProperty(PropertyName = "calling_code")]
        public string CallingCode { get; set; } = "";

        [JsonProperty(PropertyName = "is_eu")]
        public bool IsEu { get; set; }
    }
}
