namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship
{
    public class AddDraftApprenticeshipEmailViewModel
    {
        public long ProviderId { get; set; }
        public Guid? ReservationId { get; set; }
        public string Email { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long CohortId { get; set; }
        public string Name { get; set; }
        public string CohortReference { get; set; }
    }
}
