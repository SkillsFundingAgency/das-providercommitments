using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class DetailsRequest : IAuthorizationContextModel
    {
        [FromRoute]
        public long ProviderId { get; set; }
        [FromRoute]
        public string CohortReference { get; set; }
    }
}
