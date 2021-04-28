using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public interface IDraftApprenticeshipViewModel
    {
        long ProviderId { get; }
        string CohortReference { get; }
    }
}
