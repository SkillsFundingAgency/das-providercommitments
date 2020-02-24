using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Requests.Apprentice
{
    public class SelectNewEmployerRequest : IAuthorizationContextModel
    {
        public long ApprenticeshipId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ProviderId { get; set; }
    }
}
