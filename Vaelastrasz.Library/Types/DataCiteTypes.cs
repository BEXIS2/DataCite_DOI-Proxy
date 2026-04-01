using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Vaelastrasz.Library.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AccountType
    {
        [EnumMember(Value = "datacite")]
        DataCite = 1
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteCitationStyleType
    {
        [EnumMember(Value = "apa")]
        APA = 1,

        [EnumMember(Value = "harvard-cite-them-right")]
        Harvard,

        [EnumMember(Value = "modern-language-association")]
        MLA,

        [EnumMember(Value = "vancouver")]
        Vancouver,

        [EnumMember(Value = "chicago-fullnote-bibliography")]
        Chicago,

        [EnumMember(Value = "ieee")]
        IEEE,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteContributorType
    {
        [EnumMember(Value = "ContactPerson")]
        ContactPerson = 1,

        [EnumMember(Value = "DataCollector")]
        DataCollector,

        [EnumMember(Value = "DataCurator")]
        DataCurator,

        [EnumMember(Value = "DataManager")]
        DataManager,

        [EnumMember(Value = "Distributor")]
        Distributor,

        [EnumMember(Value = "Editor")]
        Editor,

        [EnumMember(Value = "HostingInstitution")]
        HostingInstitution,

        [EnumMember(Value = "Producer")]
        Producer,

        [EnumMember(Value = "ProjectLeader")]
        ProjectLeader,

        [EnumMember(Value = "ProjectManager")]
        ProjectManager,

        [EnumMember(Value = "ProjectMember")]
        ProjectMember,

        [EnumMember(Value = "RegistrationAgency")]
        RegistrationAgency,

        [EnumMember(Value = "RegistrationAuthority")]
        RegistrationAuthority,

        [EnumMember(Value = "RelatedPerson")]
        RelatedPerson,

        [EnumMember(Value = "Researcher")]
        Researcher,

        [EnumMember(Value = "ResearchGroup")]
        ResearchGroup,

        [EnumMember(Value = "RightsHolder")]
        RightsHolder,

        [EnumMember(Value = "Sponsor")]
        Sponsor,

        [EnumMember(Value = "Supervisor")]
        Supervisor,

        [EnumMember(Value = "Translator")]
        Translator,

        [EnumMember(Value = "WorkPackageLeader")]
        WorkPackageLeader,

        [EnumMember(Value = "Other")]
        Other,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteDateType
    {
        [EnumMember(Value = "Accepted")]
        Accepted = 1,

        [EnumMember(Value = "Available")]
        Available,

        [EnumMember(Value = "Copyrighted")]
        Copyrighted,

        [EnumMember(Value = "Collected")]
        Collected,

        [EnumMember(Value = "Coverage")]
        Coverage,

        [EnumMember(Value = "Created")]
        Created,

        [EnumMember(Value = "Issued")]
        Issued,

        [EnumMember(Value = "Submitted")]
        Submitted,

        [EnumMember(Value = "Updated")]
        Updated,

        [EnumMember(Value = "Valid")]
        Valid,

        [EnumMember(Value = "Withdrawn")]
        Withdrawn,

        [EnumMember(Value = "Other")]
        Other
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteDescriptionType
    {
        [EnumMember(Value = "Abstract")]
        Abstract = 1,

        [EnumMember(Value = "Methods")]
        Methods,

        [EnumMember(Value = "SeriesInformation")]
        SeriesInformation,

        [EnumMember(Value = "TableOfContents")]
        TableOfContents,

        [EnumMember(Value = "TechnicalInfo")]
        TechnicalInfo,

        [EnumMember(Value = "Other")]
        Other
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteEventType
    {
        [EnumMember(Value = "publish")]
        Publish = 1,

        [EnumMember(Value = "register")]
        Register,

        [EnumMember(Value = "hide")]
        Hide
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteFunderIdentifierType
    {
        [EnumMember(Value = "Crossref Funder ID")]
        CrossrefFunderID = 1,

        [EnumMember(Value = "GRID")]
        GRID,

        [EnumMember(Value = "ISNI")]
        ISNI,

        [EnumMember(Value = "ROR")]
        ROR,

        [EnumMember(Value = "Other")]
        Other
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteIdentifierType
    {
        [EnumMember(Value = "doi")]
        DOI = 1
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteMetadataFormatType
    {
        [EnumMember(Value = "application/vnd.datacite.datacite+xml")]
        DataCite_XML = 1,

        [EnumMember(Value = "application/vnd.datacite.datacite+json")]
        DataCite_JSON,

        [EnumMember(Value = "application/vnd.schemaorg.ld+json")]
        Schemaorg_JSONLD,

        [EnumMember(Value = "application/vnd.citationstyles.csl+json")]
        Citeproc_JSON,

        [EnumMember(Value = "application/vnd.codemeta.ld+json")]
        Codemeta_JSON,

        [EnumMember(Value = "application/x-bibtex")]
        BibTeX,

        [EnumMember(Value = "application/x-research-info-systems")]
        RIS,

        [EnumMember(Value = "application/vnd.jats+xml")]
        JATS_XML
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteNameType
    {
        [EnumMember(Value = "Personal")]
        Personal = 1,

        [EnumMember(Value = "Organizational")]
        Organizational
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteRelatedIdentifierType
    {
        [EnumMember(Value = "ARK")]
        ARK = 1,

        [EnumMember(Value = "arXiv")]
        arXiv,

        [EnumMember(Value = "bibcode")]
        bibcode,

        [EnumMember(Value = "CSTR")]
        CSTR,

        [EnumMember(Value = "DOI")]
        DOI,

        [EnumMember(Value = "EAN13")]
        EAN13,

        [EnumMember(Value = "EISSN")]
        EISSN,

        [EnumMember(Value = "Handle")]
        Handle,

        [EnumMember(Value = "IGSN")]
        IGSN,

        [EnumMember(Value = "ISBN")]
        ISBN,

        [EnumMember(Value = "ISSN")]
        ISSN,

        [EnumMember(Value = "ISTC")]
        ISTC,

        [EnumMember(Value = "LISSN")]
        LISSN,

        [EnumMember(Value = "LSID")]
        LSID,

        [EnumMember(Value = "PMID")]
        PMID,

        [EnumMember(Value = "PURL")]
        PURL,

        [EnumMember(Value = "RAiD")]
        RAiD,

        [EnumMember(Value = "RRID")]
        RRID,

        [EnumMember(Value = "SWHID")]
        SWHID,

        [EnumMember(Value = "UPC")]
        UPC,

        [EnumMember(Value = "URL")]
        URL,

        [EnumMember(Value = "URN")]
        URN,

        [EnumMember(Value = "w3id")]
        w3id
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteRelatedItemNumberType
    {
        [EnumMember(Value = "Article")]
        Article = 1,

        [EnumMember(Value = "Chapter")]
        Chapter,

        [EnumMember(Value = "Report")]
        Report,

        [EnumMember(Value = "Other")]
        Other,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteRelationType
    {
        [EnumMember(Value = "IsCitedBy")]
        IsCitedBy = 1,

        [EnumMember(Value = "Cites")]
        Cites,

        [EnumMember(Value = "IsSupplementTo")]
        IsSupplementTo,

        [EnumMember(Value = "IsSupplementedBy")]
        IsSupplementedBy,

        [EnumMember(Value = "IsContinuedBy")]
        IsContinuedBy,

        [EnumMember(Value = "Continues")]
        Continues,

        [EnumMember(Value = "IsDescribedBy")]
        IsDescribedBy,

        [EnumMember(Value = "Describes")]
        Describes,

        [EnumMember(Value = "HasMetadata")]
        HasMetadata,

        [EnumMember(Value = "IsMetadataFor")]
        IsMetadataFor,

        [EnumMember(Value = "HasVersion")]
        HasVersion,

        [EnumMember(Value = "IsVersionOf")]
        IsVersionOf,

        [EnumMember(Value = "IsNewVersionOf")]
        IsNewVersionOf,

        [EnumMember(Value = "IsPreviousVersionOf")]
        IsPreviousVersionOf,

        [EnumMember(Value = "IsPartOf")]
        IsPartOf,

        [EnumMember(Value = "HasPart")]
        HasPart,

        [EnumMember(Value = "IsPublishedIn")]
        IsPublishedIn,

        [EnumMember(Value = "IsReferencedBy")]
        IsReferencedBy,

        [EnumMember(Value = "References")]
        References,

        [EnumMember(Value = "IsDocumentedBy")]
        IsDocumentedBy,

        [EnumMember(Value = "Documents")]
        Documents,

        [EnumMember(Value = "IsCompiledBy")]
        IsCompiledBy,

        [EnumMember(Value = "Compiles")]
        Compiles,

        [EnumMember(Value = "IsVariantFormOf")]
        IsVariantFormOf,

        [EnumMember(Value = "IsOriginalFormOf")]
        IsOriginalFormOf,

        [EnumMember(Value = "IsIdenticalTo")]
        IsIdenticalTo,

        [EnumMember(Value = "IsReviewedBy")]
        IsReviewedBy,

        [EnumMember(Value = "Reviews")]
        Reviews,

        [EnumMember(Value = "IsDerivedFrom")]
        IsDerivedFrom,

        [EnumMember(Value = "IsSourceOf")]
        IsSourceOf,

        [EnumMember(Value = "IsRequiredBy")]
        IsRequiredBy,

        [EnumMember(Value = "Requires")]
        Requires,

        [EnumMember(Value = "IsObsoletedBy")]
        IsObsoletedBy,

        [EnumMember(Value = "Obsoletes")]
        Obsoletes,

        [EnumMember(Value = "IsCollectedBy")]
        IsCollectedBy,

        [EnumMember(Value = "Collects")]
        Collects,

        [EnumMember(Value = "IsTranslationOf")]
        IsTranslationOf,

        [EnumMember(Value = "HasTranslation")]
        HasTranslation,

        [EnumMember(Value = "Other")]
        Other
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteResourceTypeGeneral
    {
        [EnumMember(Value = "Audiovisual")]
        Audiovisual = 1,

        [EnumMember(Value = "Award")]
        Award,

        [EnumMember(Value = "Book")]
        Book,

        [EnumMember(Value = "BookChapter")]
        BookChapter,

        [EnumMember(Value = "Collection")]
        Collection,

        [EnumMember(Value = "ComputationalNotebook")]
        ComputationalNotebook,

        [EnumMember(Value = "ConferencePaper")]
        ConferencePaper,

        [EnumMember(Value = "ConferenceProceeding")]
        ConferenceProceeding,

        [EnumMember(Value = "DataPaper")]
        DataPaper,

        [EnumMember(Value = "Dataset")]
        Dataset,

        [EnumMember(Value = "Dissertation")]
        Dissertation,

        [EnumMember(Value = "Event")]
        Event,

        [EnumMember(Value = "Image")]
        Image,

        [EnumMember(Value = "Instrument")]
        Instrument,

        [EnumMember(Value = "InteractiveResource")]
        InteractiveResource,

        [EnumMember(Value = "JournalArticle")]
        JournalArticle,

        [EnumMember(Value = "Model")]
        Model,

        [EnumMember(Value = "OutputManagementPlan")]
        OutputManagementPlan,

        [EnumMember(Value = "PeerReview")]
        PeerReview,

        [EnumMember(Value = "PhysicalObject")]
        PhysicalObject,

        [EnumMember(Value = "Poster")]
        Poster,

        [EnumMember(Value = "Preprint")]
        Preprint,

        [EnumMember(Value = "Presentation")]
        Presentation,

        [EnumMember(Value = "Project")]
        Project,

        [EnumMember(Value = "Report")]
        Report,

        [EnumMember(Value = "Service")]
        Service,

        [EnumMember(Value = "Software")]
        Software,

        [EnumMember(Value = "Sound")]
        Sound,

        [EnumMember(Value = "Standard")]
        Standard,

        [EnumMember(Value = "StudyRegistration")]
        StudyRegistration,

        [EnumMember(Value = "Text")]
        Text,

        [EnumMember(Value = "Workflow")]
        Workflow,

        [EnumMember(Value = "Other")]
        Other
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteStateType
    {
        [EnumMember(Value = "findable")]
        Findable = 1,

        [EnumMember(Value = "registered")]
        Registered,

        [EnumMember(Value = "draft")]
        Draft
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteTitleType
    {
        [EnumMember(Value = "AlternativeTitle")]
        AlternativeTitle = 1,

        [EnumMember(Value = "Subtitle")]
        Subtitle,

        [EnumMember(Value = "TranslatedTitle")]
        TranslatedTitle,

        [EnumMember(Value = "Other")]
        Other
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataCiteType
    {
        [EnumMember(Value = "dois")]
        DOIs = 1
    }
}