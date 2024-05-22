using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ConfirmRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public Guid CacheKey { get; set; }
    }
}
