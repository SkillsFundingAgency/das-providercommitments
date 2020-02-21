using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class SelectEmployerViewModel
    {
        public long ProviderId { get; set; }
        public IList<AccountProviderLegalEntityViewModel> AccountProviderLegalEntities { get; set; }
        public long ApprenticeshipId { get; set; }
        public string ApprenticeshipHashedId { get; set; }

    }
}
