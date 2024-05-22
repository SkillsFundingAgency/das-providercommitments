using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class SentRequest : IAuthorizationContextModel
    {
        public string ApprenticeshipHashedId { get; set; }
        public long ProviderId { get; set; }
    }
}
