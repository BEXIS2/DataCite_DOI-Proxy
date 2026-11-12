using Newtonsoft.Json;

namespace Vaelastrasz.Library.Models.ORCiD
{
    public class ORCiDName
    {
        [JsonProperty("created-date")]
        public ORCiDDate CreatedDate { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        [JsonProperty("given-names")]
        public ORCiDString GivenNames { get; set; }

        [JsonProperty("family-name")]
        public ORCiDString FamilyName { get; set; }

        [JsonProperty("credit-name")]
        public ORCiDString CreditName { get; set; }

        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }
}