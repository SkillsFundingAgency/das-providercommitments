using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class SentRequest : IAuthorizationContextModel
    {
        public string ApprenticeshipHashedId { get; set; }
    }
}
