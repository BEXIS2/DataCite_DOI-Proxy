using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Reflection;

namespace Vaelastrasz.Library.Resolvers
{
    public class IgnoreEmptyCollectionsResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var shouldSerialize = property.ShouldSerialize;

            property.ShouldSerialize = instance =>
            {
                if (shouldSerialize != null && !shouldSerialize(instance))
                    return false;

                var value = property.ValueProvider.GetValue(instance);

                if (value == null)
                    return false;

                // Prüfen auf leere Collections
                if (value is IEnumerable enumerable && !(value is string))
                {
                    var enumerator = enumerable.GetEnumerator();
                    return enumerator.MoveNext(); // false = leer → wird ignoriert
                }

                return true;
            };

            return property;
        }
    }
}