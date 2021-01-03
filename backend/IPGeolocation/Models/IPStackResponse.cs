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
}
