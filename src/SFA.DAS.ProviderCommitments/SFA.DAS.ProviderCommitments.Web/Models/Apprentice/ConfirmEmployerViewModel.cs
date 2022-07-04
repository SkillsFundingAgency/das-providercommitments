using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ConfirmEmployerViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string EmployerAccountName { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long EmployerAccountLegalEntityId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string EmployerAccountLegalEntityName { get; set; }
        public bool? Confirm { get; set; }
    }
}
