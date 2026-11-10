using Newtonsoft.Json;

namespace Vaelastrasz.Library.Models.ORCiD
{
    public class ORCiDHistory
    {
        [JsonProperty("creation-method")]
        public string CreationMethod { get; set; }

        [JsonProperty("completion-date")]
        public ORCiDDate CompletionDate { get; set; }

        [JsonProperty("submission-date")]
        public ORCiDDate SubmissionDate { get; set; }

        [JsonProperty("last-modified-date")]
        public ORCiDDate LastModifiedDate { get; set; }

        [JsonProperty("claimed")]
        public bool Claimed { get; set; }

        [JsonProperty("deactivation-date")]
        public ORCiDDate DeactivationDate { get; set; }

        [JsonProperty("verified-email")]
        public bool VerifiedEmail { get; set; }

        [JsonProperty("verified-primary-email")]
        public bool VerifiedPrimaryEmail { get; set; }

        public ORCiDHistory()
        { }
    }
}