using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship
{
    public class DraftApprenticeshipSetReferenceViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public Guid? ReservationId { get; set; }
        public string Reference { get; set; }
        public string OriginalReference { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long CohortId { get; set; }
        public string Name { get; set; }
        public string CohortReference { get; set; }
        public long DraftApprenticeshipId { get; set; }

        public Party Party { get; set; }

        public string DisplayUpdateMessage()
        {
            if (string.IsNullOrEmpty(OriginalReference) && !string.IsNullOrEmpty(Reference))
                return "Reference added";
            if (!string.IsNullOrEmpty(OriginalReference) && string.IsNullOrEmpty(Reference))
                return "Reference removed";
            if (OriginalReference != Reference)
                return "Reference updated";
            return null;
        }
        public bool HasChanged() => Reference != OriginalReference;
    }
}
