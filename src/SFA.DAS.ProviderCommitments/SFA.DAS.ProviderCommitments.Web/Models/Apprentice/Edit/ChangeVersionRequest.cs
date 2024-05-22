using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class ChangeVersionRequest : IAuthorizationContextModel
    {
        public long ApprenticeshipId { get; set; }
    }
}
