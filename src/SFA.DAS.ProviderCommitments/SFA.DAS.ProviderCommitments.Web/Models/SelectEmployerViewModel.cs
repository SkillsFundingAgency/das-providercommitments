using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class SelectEmployerViewModel
    {
        public IEnumerable<AccountProviderLegalEntityViewModel> AccountProviderLegalEntities { get; set; }
        public string BackLink { get; set; }
    }
}
