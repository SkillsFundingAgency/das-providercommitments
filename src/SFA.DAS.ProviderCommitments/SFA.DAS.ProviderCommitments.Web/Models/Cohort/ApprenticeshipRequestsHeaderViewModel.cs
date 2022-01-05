namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ApprenticeshipRequestsHeaderViewModel
    {
        public long ProviderId { get; set; }
        public bool ShowDrafts { get; set; }
        public ApprenticeshipRequestsTabViewModel CohortsInDraft { get; set; }
        public ApprenticeshipRequestsTabViewModel CohortsInReview { get; set; }
        public ApprenticeshipRequestsTabViewModel CohortsWithEmployer { get; set; }
        public ApprenticeshipRequestsTabViewModel CohortsWithTransferSender { get; set; }
        public bool IsAgreementSigned { get; set; }
    }
}
