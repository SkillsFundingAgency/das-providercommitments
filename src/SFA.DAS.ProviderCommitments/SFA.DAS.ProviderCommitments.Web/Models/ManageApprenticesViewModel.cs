using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ManageApprenticesViewModel
    {
        public long ProviderId { get; set; }
        public bool AnyApprenticeships => Apprenticeships != null && Apprenticeships.Any();
        public IEnumerable<ApprenticeshipDetails> Apprenticeships { get; set; }

        public ManageApprenticesFilterModel FilterModel { get; set; }
        public bool ShowPageLinks  => FilterModel.TotalNumberOfApprenticeshipsFound > ProviderCommitmentsWebConstants.NumberOfApprenticesPerSearchPage;
    }
}