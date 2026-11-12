using Newtonsoft.Json;
using System;

namespace Vaelastrasz.Library.Models.ORCiD
{
    public class ORCiDIdentifier
    {
        public ORCiDIdentifier()
        { }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }
    }
}