using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class EditApprenticeshipRequest : BaseApprenticeshipRequest,IAuthorizationContextModel
    {
        [FromRoute]
        public long ApprenticeshipId { get; set; }
    }
    
    public class BaseApprenticeshipRequest
    {
        [FromRoute]
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
    }
}
