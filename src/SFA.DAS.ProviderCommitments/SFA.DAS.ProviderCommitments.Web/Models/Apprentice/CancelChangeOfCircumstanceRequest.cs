using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class CancelChangeOfCircumstanceRequest : IAuthorizationContextModel
    {
        [FromRoute]
        public long ProviderId { get; set; }

        [FromRoute]
        public string ApprenticeshipHashedId { get; set; }
    }
}
