using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class SelectIlrRecordRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string SearchTerm { get; set; }
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
    }
}
