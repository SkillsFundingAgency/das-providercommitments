using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class DeleteCohortRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }

        public string CohortReference { get; set; }

        public long CohortId { get; set; }
    }
}
