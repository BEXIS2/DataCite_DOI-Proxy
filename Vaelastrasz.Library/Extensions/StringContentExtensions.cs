using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Vaelastrasz.Library.Resolvers;

namespace Vaelastrasz.Library.Extensions
{
    public static class StringContentExtensions
    {
        public static StringContent AsJson(this object o) => new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");

        public static StringContent AsMinimalJson(this object o)
        {
            var json = JsonConvert.SerializeObject(o, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
                //ContractResolver = new IgnoreEmptyCollectionsResolver()
            });

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}