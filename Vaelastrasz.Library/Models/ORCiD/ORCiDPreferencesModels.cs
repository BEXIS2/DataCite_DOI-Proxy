using Newtonsoft.Json;

namespace Vaelastrasz.Library.Models.ORCiD
{
    public class ORCiDPreferences
    {
        public ORCiDPreferences()
        { }

        [JsonProperty("locale")]
        public string Locale { get; set; }
    }
}