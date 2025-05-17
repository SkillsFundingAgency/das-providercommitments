using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ConfirmEmployerViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public bool UseLearnerData { get; set; }
        public string EmployerAccountName { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string EmployerAccountLegalEntityName { get; set; }
        public bool? Confirm { get; set; }
    }
}
