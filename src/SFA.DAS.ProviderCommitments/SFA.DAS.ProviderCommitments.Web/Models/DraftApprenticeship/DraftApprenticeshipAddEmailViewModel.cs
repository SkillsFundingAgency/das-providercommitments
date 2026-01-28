using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship
{
    public class DraftApprenticeshipAddEmailViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string Email { get; set; }
        public string OriginalEmail { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long CohortId { get; set; }
        public string Name { get; set; }
        public string CohortReference { get; set; }
        public long DraftApprenticeshipId { get; set; }
        public string DisplayUpdateMessage()
        {
            if (string.IsNullOrEmpty(OriginalEmail) && !string.IsNullOrEmpty(Email))
                return "Email added";
            if (!string.IsNullOrEmpty(OriginalEmail) && string.IsNullOrEmpty(Email))
                return "Email removed";
            if (OriginalEmail != Email)
                return "Email updated";
            return null;
        }
        public bool HasChanged() => Email != OriginalEmail;
    }
}
