using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ManageApprenticesViewModel
    {
        public long ProviderId { get; set; }
        public bool AnyApprenticeships => Apprenticeships.Any();
        public IEnumerable<ApprenticeshipDetails> Apprenticeships { get; set; }
    }
}