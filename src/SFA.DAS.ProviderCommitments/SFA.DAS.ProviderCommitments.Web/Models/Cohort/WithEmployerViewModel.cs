using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class WithEmployerViewModel
    {
        public long ProviderId { get; set; }
        public IEnumerable<WithEmployerSummaryViewModel> Cohorts { get; set; }
    }
}
