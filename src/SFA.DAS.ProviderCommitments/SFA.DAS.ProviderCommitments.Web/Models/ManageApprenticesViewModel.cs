using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;


namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ManageApprenticesViewModel
    {
        public long? ProviderId { get; set; }
        public bool AnyApprenticeships => Apprenticeships != null && Apprenticeships.Any();

        public IEnumerable<ApprenticeshipDetailsViewModel> Apprenticeships { get; set; }
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
        public string SortedByHeaderClassName { get; set; }
        public const string HeaderClassName = "das-table__sort";
    
        public void SortedByHeader()
        {
            SortedByHeaderClassName += HeaderClassName;
            if (ReverseSort)
            {
                SortedByHeaderClassName += " das-table__sort--desc";
            }
            else
            {
                SortedByHeaderClassName += " das-table__sort--asc";
            }
        }

        public ManageApprenticesFilterModel FilterModel { get; set; }
        public bool ShowPageLinks  => FilterModel.TotalNumberOfApprenticeshipsFound > ProviderCommitmentsWebConstants.NumberOfApprenticesPerSearchPage;
    }
}