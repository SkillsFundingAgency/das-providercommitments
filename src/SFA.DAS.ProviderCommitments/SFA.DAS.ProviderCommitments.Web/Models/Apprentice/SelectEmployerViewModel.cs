﻿using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class SelectEmployerViewModel
    {
        public IList<AccountProviderLegalEntityViewModel> AccountProviderLegalEntities { get; set; }
        public string LegalEntityName { get; set; }
        public SelectEmployerFilterModel SelectEmployerFilterModel { get; set; }
    }
}
