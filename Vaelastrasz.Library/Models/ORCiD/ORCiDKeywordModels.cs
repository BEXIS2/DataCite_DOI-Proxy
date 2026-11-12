using Newtonsoft.Json;
using System.Collections.Generic;

namespace Vaelastrasz.Library.Models.ORCiD
{
    public class ORCiDKeywords
    {
        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        [JsonProperty("keyword")]
        public List<string> Keyword { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }
}