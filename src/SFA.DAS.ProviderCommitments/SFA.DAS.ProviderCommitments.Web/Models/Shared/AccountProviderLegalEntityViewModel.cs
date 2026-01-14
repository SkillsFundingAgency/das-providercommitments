using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Shared
{
    public class AccountProviderLegalEntityViewModel
    {
        public string EmployerAccountPublicHashedId { get; set; }
        public string EmployerAccountName { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public string EmployerAccountLegalEntityName { get; set; }
        public string SelectEmployerUrl { get; set; }
        public string AccountHashedId { get; set; }
        public ApprenticeshipEmployerType LevyStatus { get; set; }
    }
}
