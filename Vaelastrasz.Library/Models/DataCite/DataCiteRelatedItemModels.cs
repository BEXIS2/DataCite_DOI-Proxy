using Newtonsoft.Json;
using System.Collections.Generic;
using Vaelastrasz.Library.Types;

namespace Vaelastrasz.Library.Models.DataCite
{
    public class DataCiteRelatedItem
    {
        public DataCiteRelatedItem()
        {
            Creators = new List<DataCiteRelatedItemCreator>();
            Titles = new List<DataCiteRelatedItemTitle>();
            Contributors = new List<DataCiteRelatedItemContributor>();
        }

        [JsonProperty("contributors")]
        public List<DataCiteRelatedItemContributor> Contributors { get; set; }

        [JsonProperty("creators")]
        public List<DataCiteRelatedItemCreator> Creators { get; set; }

        [JsonProperty("edition")]
        public string Edition { get; set; }

        [JsonProperty("firstPage")]
        public string FirstPage { get; set; }

        [JsonProperty("issue")]
        public string Issue { get; set; }

        [JsonProperty("lastPage")]
        public string LastPage { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("numberType")]
        public DataCiteRelatedItemNumberType? NumberType { get; set; }

        [JsonProperty("publicationYear")]
        public string PublicationYear { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [JsonProperty("relatedItemIdentifier")]
        public DataCiteRelationType? RelatedItemIdentifier { get; set; }

        [JsonProperty("relatedItemType")]
        public DataCiteResourceTypeGeneral RelatedItemType { get; set; }

        [JsonProperty("relationType")]
        public DataCiteRelationType RelationType { get; set; }

        [JsonProperty("relationTypeInformation")]
        public string RelationTypeInformation { get; set; }

        [JsonProperty("titles")]
        public List<DataCiteRelatedItemTitle> Titles { get; set; }

        [JsonProperty("volume")]
        public string Volume { get; set; }
    }
}