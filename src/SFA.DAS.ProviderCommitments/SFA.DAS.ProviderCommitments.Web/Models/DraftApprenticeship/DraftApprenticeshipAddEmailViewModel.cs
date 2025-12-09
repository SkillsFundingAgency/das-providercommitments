using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship
{
    public class DraftApprenticeshipAddEmailViewModel
    {
        public long ProviderId { get; set; }
        public Guid? ReservationId { get; set; }

        public string Email { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long CohortId { get; set; }
        public string Name { get; set; }
        public string CohortReference { get; set; }
        public long DraftApprenticeshipId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsEdit { get; set; }
    }
}
