using Newtonsoft.Json;
using System;

namespace DerMistkaefer.DvbLive.IPGeolocation.Models
{
    internal class IpStackResponse
    {
        [JsonProperty(PropertyName = "ip")]
        public string Ip { get; set; } = "";

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "";

        [JsonProperty(PropertyName = "continent_code")]
        public string ContinentCode { get; set; } = "";

        [JsonProperty(PropertyName = "continent_name")]
        public string ContinentName { get; set; } = "";

        [JsonProperty(PropertyName = "region_code")]
        public string RegionCode { get; set; } = "";

        [JsonProperty(PropertyName = "region_name")]
        public string RegionName { get; set; } = "";

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; } = "";

        [JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; } = "";

        [JsonProperty(PropertyName = "latitude")]
        public decimal Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public decimal Longitude { get; set; }

        [JsonProperty(PropertyName = "location")]
        public IpStackLocationResponse Location { get; set; } = new();
    }

    internal class IpStackLocationResponse
    {
        [JsonProperty(PropertyName = "geoname_id")]
        public int GeonameId { get; set; }

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
