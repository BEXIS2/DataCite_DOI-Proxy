using Newtonsoft.Json;

namespace Vaelastrasz.Library.Models.ORCiD
{
    public class ORCiDString
    {
        public ORCiDString()
        { }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class ORCiDInteger
    {
        public ORCiDInteger()
        { }

        [JsonProperty("value")]
        public int Value { get; set; }
    }

    public class ORCiDDate
    {
        public ORCiDDate()
        { }

        [JsonProperty("value")]
        public long Value { get; set; }
    }
}