using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class SelectEmployerViewModel
    {
        public long ProviderId { get; set; }
        public bool UseIlrData { get; set; }
        public IList<AccountProviderLegalEntityViewModel> AccountProviderLegalEntities { get; set; }
        public string BackLink { get; set; }
        public SelectEmployerFilterModel SelectEmployerFilterModel { get; set; }
    }
}
