using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class SelectEmployerRequest : IAuthorizationContextModel
    {
        public long ApprenticeshipId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ProviderId { get; set; }
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
        public string SearchTerm { get; set; }
        public Guid CacheKey { get; set; }
    }
}
