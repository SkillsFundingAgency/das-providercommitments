using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class SelectEmployerViewModel
    {
        public long ProviderId { get; set; }
        public IList<AccountProviderLegalEntityViewModel> AccountProviderLegalEntities { get; set; }
        public string BackLink { get; set; }
    }
}
