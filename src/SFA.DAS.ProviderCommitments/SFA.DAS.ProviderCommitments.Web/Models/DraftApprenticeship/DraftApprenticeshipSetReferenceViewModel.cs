using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship
{
    public class DraftApprenticeshipSetReferenceViewModel
    {
        public long ProviderId { get; set; }
        public Guid? ReservationId { get; set; }
        public string Reference { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long CohortId { get; set; }
        public string Name { get; set; }
        public string CohortReference { get; set; }
        public long DraftApprenticeshipId { get; set; }

        public Party Party { get; set; }
    }
}
