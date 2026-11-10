using Newtonsoft.Json;

namespace Vaelastrasz.Library.Models.ORCiD
{
    public class ORCiDSource
    {
        [JsonProperty("source-orcid")]
        public ORCiDIdentifier SourceORCiD { get; set; }

        [JsonProperty("source-name")]
        public ORCiDString SourceName { get; set; }
    }
}