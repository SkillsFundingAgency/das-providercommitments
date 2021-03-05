using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ChangeEmployerRequest : IAuthorizationContextModel
    {
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public long ProviderId { get; set; }
    }
}