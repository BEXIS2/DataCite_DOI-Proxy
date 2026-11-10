using Newtonsoft.Json;

namespace Vaelastrasz.Library.Models.ORCiD
{
    public class ORCiDAddresses
    {
        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        [JsonProperty("address")]
        public ORCiDAddress Address { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }

    public class ORCiDAddress
    {
        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        [JsonProperty("created-date")]
        public ORCiDDate CreatedDate { get; set; }

        [JsonProperty("country")]
        public ORCiDString Country { get; set; }

        [JsonProperty("visibility")]
        public string Visibility { get; set; }
    }
}