using Newtonsoft.Json;
using System.Collections.Generic;
using Vaelastrasz.Library.Models.ORCiD;

namespace Vaelastrasz.Library.Models
{
    #region read

    public class ReadORCiDModel
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("orcid-identifier")]
        public ORCiDIdentifier ORCiDIdentifier { get; set; }

        [JsonProperty("preferences")]
        public Preferences Preferences { get; set; }

        [JsonProperty("history")]
        public ORCiDHistory History { get; set; }

        [JsonProperty("person")]
        public ORCiDPerson Person { get; set; }

        public ReadORCiDModel()
        {
        }
    }

    #endregion read

    public class ActivitiesSummary
    {
        public Distinctions distinctions { get; set; }

        public Educations educations { get; set; }

        public Employments employments { get; set; }

        public Fundings fundings { get; set; }

        [JsonProperty("invited-positions")]
        public InvitedPositions invitedpositions { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public Memberships memberships { get; set; }

        public string path { get; set; }

        [JsonProperty("peer-reviews")]
        public PeerReviews peerreviews { get; set; }

        public Qualifications qualifications { get; set; }

        [JsonProperty("research-resources")]
        public ResearchResources researchresources { get; set; }

        public Services services { get; set; }
        public Works works { get; set; }
    }

    public class Address
    {
        public string city { get; set; }

        public Country country { get; set; }

        [JsonProperty("created-date")]
        public CreatedDate createddate { get; set; }

        [JsonProperty("display-index")]
        public int displayindex { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public string path { get; set; }

        [JsonProperty("put-code")]
        public int putcode { get; set; }

        public string region { get; set; }
        public Source source { get; set; }
        public string visibility { get; set; }
    }

    public class Addresses
    {
        public List<Address> address { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public string path { get; set; }
    }

    public class AffiliationGroup
    {
        [JsonProperty("external-ids")]
        public ExternalIds externalids { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public List<Summary> summaries { get; set; }
    }

    public class Country
    {
        public string value { get; set; }
    }

    public class CreatedDate
    {
        public long value { get; set; }
    }

    public class Day
    {
        public string value { get; set; }
    }

    public class DisambiguatedOrganization
    {
        [JsonProperty("disambiguated-organization-identifier")]
        public string disambiguatedorganizationidentifier { get; set; }

        [JsonProperty("disambiguation-source")]
        public string disambiguationsource { get; set; }
    }

    public class Distinctions
    {
        [JsonProperty("affiliation-group")]
        public List<object> affiliationgroup { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class Educations
    {
        [JsonProperty("affiliation-group")]
        public List<AffiliationGroup> affiliationgroup { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public string path { get; set; }
    }

    public class EducationSummary
    {
        [JsonProperty("created-date")]
        public CreatedDate createddate { get; set; }

        [JsonProperty("department-name")]
        public object departmentname { get; set; }

        [JsonProperty("display-index")]
        public string displayindex { get; set; }

        [JsonProperty("end-date")]
        public EndDate enddate { get; set; }

        [JsonProperty("external-ids")]
        public object externalids { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public Organization organization { get; set; }
        public string path { get; set; }

        [JsonProperty("put-code")]
        public int putcode { get; set; }

        [JsonProperty("role-title")]
        public string roletitle { get; set; }

        public Source source { get; set; }

        [JsonProperty("start-date")]
        public StartDate startdate { get; set; }

        public object url { get; set; }
        public string visibility { get; set; }
    }

    public class Emails
    {
        public List<object> email { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class Employments
    {
        [JsonProperty("affiliation-group")]
        public List<AffiliationGroup> affiliationgroup { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public string path { get; set; }
    }

    public class EmploymentSummary
    {
        [JsonProperty("created-date")]
        public CreatedDate createddate { get; set; }

        [JsonProperty("department-name")]
        public object departmentname { get; set; }

        [JsonProperty("display-index")]
        public string displayindex { get; set; }

        [JsonProperty("end-date")]
        public EndDate enddate { get; set; }

        [JsonProperty("external-ids")]
        public object externalids { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public Organization organization { get; set; }
        public string path { get; set; }

        [JsonProperty("put-code")]
        public int putcode { get; set; }

        [JsonProperty("role-title")]
        public object roletitle { get; set; }

        public Source source { get; set; }

        [JsonProperty("start-date")]
        public StartDate startdate { get; set; }

        public object url { get; set; }
        public string visibility { get; set; }
    }

    public class EndDate
    {
        public Day day { get; set; }
        public Month month { get; set; }
        public Year year { get; set; }
    }

    public class ExternalIdentifiers
    {
        [JsonProperty("external-identifier")]
        public List<object> externalidentifier { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class ExternalIds
    {
        [JsonProperty("external-id")]
        public List<object> externalid { get; set; }
    }

    public class FamilyName
    {
        public string value { get; set; }
    }

    public class Fundings
    {
        public List<object> group { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class GivenNames
    {
        public string value { get; set; }
    }

    public class History
    {
        public bool claimed { get; set; }

        [JsonProperty("completion-date")]
        public object completiondate { get; set; }

        [JsonProperty("creation-method")]
        public string creationmethod { get; set; }

        [JsonProperty("deactivation-date")]
        public object deactivationdate { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public object source { get; set; }

        [JsonProperty("submission-date")]
        public SubmissionDate submissiondate { get; set; }

        [JsonProperty("verified-email")]
        public bool verifiedemail { get; set; }

        [JsonProperty("verified-primary-email")]
        public bool verifiedprimaryemail { get; set; }
    }

    public class InvitedPositions
    {
        [JsonProperty("affiliation-group")]
        public List<object> affiliationgroup { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class Keywords
    {
        public List<object> keyword { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class Memberships
    {
        [JsonProperty("affiliation-group")]
        public List<object> affiliationgroup { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class Month
    {
        public string value { get; set; }
    }

    public class Name
    {
        [JsonProperty("created-date")]
        public CreatedDate createddate { get; set; }

        [JsonProperty("credit-name")]
        public object creditname { get; set; }

        [JsonProperty("family-name")]
        public FamilyName familyname { get; set; }

        [JsonProperty("given-names")]
        public GivenNames givennames { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public string path { get; set; }
        public object source { get; set; }
        public string visibility { get; set; }
    }

    public class OrcidIdentifier
    {
        public string host { get; set; }
        public string path { get; set; }
        public string uri { get; set; }
    }

    public class ORCIDModel
    {
        [JsonProperty("activities-summary")]
        public ActivitiesSummary activitiessummary { get; set; }

        public History history { get; set; }

        [JsonProperty("orcid-identifier")]
        public OrcidIdentifier orcididentifier { get; set; }

        public string path { get; set; }
        public ORCIDPerson person { get; set; }
        public Preferences preferences { get; set; }
    }

    public class ORCIDPerson
    {
        public Addresses addresses { get; set; }

        public object biography { get; set; }

        public Emails emails { get; set; }

        [JsonProperty("external-identifiers")]
        public ExternalIdentifiers externalidentifiers { get; set; }

        public Keywords keywords { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public Name name { get; set; }

        [JsonProperty("other-names")]
        public OtherNames othernames { get; set; }

        public string path { get; set; }

        [JsonProperty("researcher-urls")]
        public ResearcherUrls researcherurls { get; set; }
    }

    public class Organization
    {
        public Address address { get; set; }

        [JsonProperty("disambiguated-organization")]
        public DisambiguatedOrganization disambiguatedorganization { get; set; }

        public string name { get; set; }
    }

    public class OtherName
    {
        public string content { get; set; }

        [JsonProperty("created-date")]
        public CreatedDate createddate { get; set; }

        [JsonProperty("display-index")]
        public int displayindex { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        public string path { get; set; }

        [JsonProperty("put-code")]
        public int putcode { get; set; }

        public Source source { get; set; }
        public string visibility { get; set; }
    }

    public class OtherNames
    {
        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        [JsonProperty("other-name")]
        public List<OtherName> othername { get; set; }

        public string path { get; set; }
    }

    public class PeerReviews
    {
        public List<object> group { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class Preferences
    {
        public string locale { get; set; }
    }

    public class Qualifications
    {
        [JsonProperty("affiliation-group")]
        public List<object> affiliationgroup { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class ResearcherUrls
    {
        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }

        [JsonProperty("researcher-url")]
        public List<object> researcherurl { get; set; }
    }

    public class ResearchResources
    {
        public List<object> group { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class Services
    {
        [JsonProperty("affiliation-group")]
        public List<object> affiliationgroup { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class Source
    {
        [JsonProperty("assertion-origin-client-id")]
        public object assertionoriginclientid { get; set; }

        [JsonProperty("assertion-origin-name")]
        public object assertionoriginname { get; set; }

        [JsonProperty("assertion-origin-orcid")]
        public object assertionoriginorcid { get; set; }

        [JsonProperty("source-client-id")]
        public object sourceclientid { get; set; }

        [JsonProperty("source-name")]
        public SourceName sourcename { get; set; }

        [JsonProperty("source-orcid")]
        public SourceOrcid sourceorcid { get; set; }
    }

    public class SourceName
    {
        public string value { get; set; }
    }

    public class SourceOrcid
    {
        public string host { get; set; }
        public string path { get; set; }
        public string uri { get; set; }
    }

    public class StartDate
    {
        public Day day { get; set; }
        public Month month { get; set; }
        public Year year { get; set; }
    }

    public class SubmissionDate
    {
        public long value { get; set; }
    }

    public class Summary
    {
        [JsonProperty("education-summary")]
        public EducationSummary educationsummary { get; set; }

        [JsonProperty("employment-summary")]
        public EmploymentSummary employmentsummary { get; set; }
    }

    public class Works
    {
        public List<object> group { get; set; }

        [JsonProperty("last-modified-date")]
        public object lastmodifieddate { get; set; }

        public string path { get; set; }
    }

    public class Year
    {
        public string value { get; set; }
    }
}