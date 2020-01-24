using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ManageApprenticesViewModel
    {
        public long? ProviderId { get; set; }
        public bool AnyApprenticeships => Apprenticeships != null && Apprenticeships.Any();
        public IEnumerable<ApprenticeshipDetailsViewModel> Apprenticeships { get; set; }

        public ManageApprenticesFilterModel FilterModel { get; set; }
        public bool ShowPageLinks  => FilterModel.TotalNumberOfApprenticeshipsFound > ProviderCommitmentsWebConstants.NumberOfApprenticesPerSearchPage;
    }
}