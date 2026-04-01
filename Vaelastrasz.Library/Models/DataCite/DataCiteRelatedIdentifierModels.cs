using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Vaelastrasz.Library.Types;

namespace Vaelastrasz.Library.Models.DataCite
{
    public class DataCiteRelatedIdentifier
    {
        public DataCiteRelatedIdentifier()
        { }

        [Required]
        [JsonProperty("relatedIdentifier")]
        public string RelatedIdentifier { get; set; }

        [Required]
        [JsonProperty("relatedIdentifierType")]
        public DataCiteRelatedIdentifierType? RelatedIdentifierType { get; set; }

        [JsonProperty("relatedMetadataScheme")]
        public string RelatedMetadataScheme { get; set; }

        [Required]
        [JsonProperty("relationType")]
        public DataCiteRelationType RelationType { get; set; }

        [JsonProperty("relationTypeInformation")]
        public string RelationTypeInformation { get; set; }

        [JsonProperty("resourceTypeGeneral")]
        public DataCiteResourceTypeGeneral ResourceTypeGeneral { get; set; }

        [JsonProperty("schemeType")]
        public string SchemeType { get; set; }

        [JsonProperty("schemeUri")]
        public string SchemeUri { get; set; }
    }
}