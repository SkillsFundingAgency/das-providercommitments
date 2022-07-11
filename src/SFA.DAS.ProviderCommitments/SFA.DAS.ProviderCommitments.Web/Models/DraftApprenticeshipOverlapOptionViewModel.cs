namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class DraftApprenticeshipOverlapOptionViewModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public OverlapOptions? OverlapOptions { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long? DraftApprenticeshipId { get; set; }
    }

    public enum OverlapOptions
    {
        SendStopRequest,
        ContactTheEmployer,
        AddApprenticeshipLater
    }
}
