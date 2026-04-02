using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vaelastrasz.Library.Attributes;
using Vaelastrasz.Library.Models.DataCite;
using Vaelastrasz.Library.Types;

namespace Vaelastrasz.Library.Models
{
    #region create

    public class CreateDataCiteAttributesModel
    {
        public CreateDataCiteAttributesModel()
        {
            GeoLocations = new List<DataCiteGeoLocation>();
            AlternateIdentifiers = new List<DataCiteAlternateIdentifier>();
            Creators = new List<DataCiteCreator>();
            Contributors = new List<DataCiteContributor>();
            Dates = new List<DataCiteDate>();
            Descriptions = new List<DataCiteDescription>();
            Identifiers = new List<DataCiteIdentifier>();
            Subjects = new List<DataCiteSubject>();
            Titles = new List<DataCiteTitle>();
            RightsList = new List<DataCiteRight>();
            Sizes = new List<string>();
            Formats = new List<string>();
            RelatedIdentifiers = new List<DataCiteRelatedIdentifier>();
            FundingReferences = new List<DataCiteFundingReference>();
        }

        // 12 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/alternateidentifier/
        [JsonProperty("alternateIdentifiers")]
        public List<DataCiteAlternateIdentifier> AlternateIdentifiers { get; set; }

        // 7 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/contributor/
        [JsonProperty("contributors")]
        public List<DataCiteContributor> Contributors { get; set; }

        // 2 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/creator/
        [Required]
        [Cardinality(Minimum = 1)]
        [JsonProperty("creators")]
        public List<DataCiteCreator> Creators { get; set; }

        // 8 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/date/
        [JsonProperty("dates")]
        public List<DataCiteDate> Dates { get; set; }

        // 17 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/description/
        [JsonProperty("descriptions")]
        public List<DataCiteDescription> Descriptions { get; set; }

        // 0
        [Required]
        [JsonProperty("doi")]
        public string Doi { get; set; }

        // 0
        [Required]
        [JsonProperty("event")]
        public DataCiteEventType Event { get; set; }

        // 14 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/format/
        [JsonProperty("formats")]
        public List<string> Formats { get; set; }

        // 19 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/fundingreference/
        [JsonProperty("fundingReferences")]
        public List<DataCiteFundingReference> FundingReferences { get; set; }

        // 18 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/geolocation/
        [JsonProperty("geoLocations")]
        public List<DataCiteGeoLocation> GeoLocations { get; set; }

        // 1 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/identifier/
        [JsonProperty("identifiers")]
        public List<DataCiteIdentifier> Identifiers { get; set; }

        // 9 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/language/
        [JsonProperty("language")]
        public string Language { get; set; }

        // 5 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/publicationyear/
        [Required]
        [JsonProperty("publicationYear")]
        public int PublicationYear { get; set; }

        // 4 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/publisher/
        [Required]
        [JsonProperty("publisher")]
        public DataCitePublisher Publisher { get; set; }

        // 12 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/relatedidentifier/
        [JsonProperty("relatedIdentifiers")]
        public List<DataCiteRelatedIdentifier> RelatedIdentifiers { get; set; }

        // 16 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/rights/#
        [JsonProperty("rightsList")]
        public List<DataCiteRight> RightsList { get; set; }

        // 13 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/size/
        [JsonProperty("sizes")]
        public List<string> Sizes { get; set; }

        // 6 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/subject/
        [JsonProperty("subjects")]
        public List<DataCiteSubject> Subjects { get; set; }

        // 3 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/title/
        [Required]
        [Cardinality(Minimum = 1)]
        [JsonProperty("titles")]
        public List<DataCiteTitle> Titles { get; set; }

        // 10 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/resourcetype/
        [Required]
        [JsonProperty("types")]
        public DataCiteTypes Types { get; set; }

        // 0
        [Required]
        [JsonProperty("url")]
        public string URL { get; set; }

        // 15 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/version/
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class CreateDataCiteDataModel
    {
        public CreateDataCiteDataModel()
        {
            Attributes = new CreateDataCiteAttributesModel();
        }

        [Required]
        [JsonProperty("attributes")]
        public CreateDataCiteAttributesModel Attributes { get; set; }

        [Required]
        [JsonProperty("type")]
        public DataCiteType Type { get; set; }
    }

    public class CreateDataCiteModel
    {
        public CreateDataCiteModel()
        {
            Data = new CreateDataCiteDataModel();
        }

        [Required]
        [JsonProperty("data")]
        public CreateDataCiteDataModel Data { get; set; }
    }

    #endregion create

    #region read

    public class ReadDataCiteAttributesModel
    {
        public ReadDataCiteAttributesModel()
        {
            GeoLocations = new List<DataCiteGeoLocation>();
            AlternateIdentifiers = new List<DataCiteAlternateIdentifier>();
            Creators = new List<DataCiteCreator>();
            Contributors = new List<DataCiteContributor>();
            Dates = new List<DataCiteDate>();
            Descriptions = new List<DataCiteDescription>();
            Identifiers = new List<DataCiteIdentifier>();
            Subjects = new List<DataCiteSubject>();
            Titles = new List<DataCiteTitle>();
            RightsList = new List<DataCiteRight>();
            Sizes = new List<string>();
            Formats = new List<string>();
            RelatedIdentifiers = new List<DataCiteRelatedIdentifier>();
            FundingReferences = new List<DataCiteFundingReference>();
        }

        // 12 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/alternateidentifier/
        [JsonProperty("alternateIdentifiers")]
        public List<DataCiteAlternateIdentifier> AlternateIdentifiers { get; set; }

        // 7 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/contributor/
        [JsonProperty("contributors")]
        public List<DataCiteContributor> Contributors { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        // 2 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/creator/
        [Cardinality(Minimum = 1)]
        [JsonProperty("creators")]
        public List<DataCiteCreator> Creators { get; set; }

        // 8 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/date/
        [JsonProperty("dates")]
        public List<DataCiteDate> Dates { get; set; }

        // 17 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/description/
        [JsonProperty("descriptions")]
        public List<DataCiteDescription> Descriptions { get; set; }

        // 0
        [Required]
        [JsonProperty("doi")]
        public string Doi { get; set; }

        [JsonProperty("downloadCount")]
        public long DownloadCount { get; set; }

        // 14 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/format/
        [JsonProperty("formats")]
        public List<string> Formats { get; set; }

        // 19 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/fundingreference/
        [JsonProperty("fundingReferences")]
        public List<DataCiteFundingReference> FundingReferences { get; set; }

        // 18 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/geolocation/
        [JsonProperty("geoLocations")]
        public List<DataCiteGeoLocation> GeoLocations { get; set; }

        // 1 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/identifier/
        [JsonProperty("identifiers")]
        public List<DataCiteIdentifier> Identifiers { get; set; }

        // 9 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/language/
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        // 5 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/publicationyear/
        [Required]
        [JsonProperty("publicationYear")]
        public int PublicationYear { get; set; }

        [JsonProperty("published")]
        public string Published { get; set; }

        // 4 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/publisher/
        [Required]
        [JsonProperty("publisher")]
        public DataCitePublisher Publisher { get; set; }

        [JsonProperty("referenceCount")]
        public long ReferenceCount { get; set; }

        [JsonProperty("registered")]
        public string Registered { get; set; }

        // 12 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/relatedidentifier/
        [JsonProperty("relatedIdentifiers")]
        public List<DataCiteRelatedIdentifier> RelatedIdentifiers { get; set; }

        // 16 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/rights/#
        [JsonProperty("rightsList")]
        public List<DataCiteRight> RightsList { get; set; }

        // 13 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/size/
        [JsonProperty("sizes")]
        public List<string> Sizes { get; set; }

        [JsonProperty("state")]
        public DataCiteStateType State { get; set; }

        // 6 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/subject/
        [JsonProperty("subjects")]
        public List<DataCiteSubject> Subjects { get; set; }

        [JsonProperty("suffix")]
        public string Suffix { get; set; }

        // 3 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/title/
        [Cardinality(Minimum = 1)]
        [JsonProperty("titles")]
        public List<DataCiteTitle> Titles { get; set; }

        // 10 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/resourcetype/
        [Required]
        [JsonProperty("types")]
        public DataCiteTypes Types { get; set; }

        [JsonProperty("updated")]
        public string Updated { get; set; }

        // 0
        [Required]
        [JsonProperty("url")]
        public string URL { get; set; }

        // 15 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/version/
        [JsonProperty("version")]
        public string Version { get; set; }

        //
        [JsonProperty("viewCount")]
        public long ViewCount { get; set; }
    }

    public class ReadDataCiteDataModel
    {
        public ReadDataCiteDataModel()
        {
            Attributes = new ReadDataCiteAttributesModel();
        }

        [JsonProperty("attributes")]
        public ReadDataCiteAttributesModel Attributes { get; set; }

        [JsonProperty("type")]
        public DataCiteType Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class ReadDataCiteModel
    {
        public ReadDataCiteModel()
        {
            Data = new ReadDataCiteDataModel();
        }

        [JsonProperty("data")]
        public ReadDataCiteDataModel Data { get; set; }
    }

    #endregion read

    #region update

    public class UpdateDataCiteAttributesModel
    {
        [JsonProperty("contributors")]
        public List<DataCiteCreator> Contributors { get; set; }

        [JsonProperty("creators")]
        public List<DataCiteCreator> Creators { get; set; }

        [JsonProperty("dates")]
        public List<DataCiteDate> Dates { get; set; }

        [JsonProperty("descriptions")]
        public List<DataCiteDescription> Descriptions { get; set; }

        [JsonProperty("doi")]
        public string Doi { get; set; }

        [JsonProperty("event")]
        public DataCiteEventType Event { get; set; }

        [JsonProperty("identifiers")]
        public List<DataCiteIdentifier> Identifiers { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("publicationYear")]
        public int PublicationYear { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [JsonProperty("relatedIdentifiers")]
        public List<DataCiteRelatedIdentifier> RelatedIdentifiers { get; set; }

        // 16 - https://datacite-metadata-schema.readthedocs.io/en/4.5/properties/rights/#
        [JsonProperty("rightsList")]
        public List<DataCiteRight> RightsList { get; set; }

        [JsonProperty("subjects")]
        public List<DataCiteSubject> Subjects { get; set; }

        [JsonProperty("titles")]
        public List<DataCiteTitle> Titles { get; set; }

        [JsonProperty("types")]
        public DataCiteTypes Types { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class UpdateDataCiteDataModel
    {
        public UpdateDataCiteDataModel()
        {
            Attributes = new UpdateDataCiteAttributesModel();
        }

        [JsonProperty("attributes")]
        public UpdateDataCiteAttributesModel Attributes { get; set; }

        [Required]
        [JsonProperty("type")]
        public DataCiteType Type { get; set; }
    }

    public class UpdateDataCiteModel
    {
        public UpdateDataCiteModel()
        {
            Data = new UpdateDataCiteDataModel();
        }

        [JsonProperty("data")]
        public UpdateDataCiteDataModel Data { get; set; }
    }

    #endregion update
}