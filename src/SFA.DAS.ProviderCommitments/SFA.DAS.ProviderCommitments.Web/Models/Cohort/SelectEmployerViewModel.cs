using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class SelectEmployerViewModel
    {
        public long ProviderId { get; set; }
        public IList<AccountProviderLegalEntityViewModel> AccountProviderLegalEntities { get; set; }
        public string BackLink { get; set; }
        public SelectEmployerFilterModel SelectEmployerFilterModel { get; set; }
    }

    public class SelectEmployerFilterModel
    {
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }

        public Dictionary<string, string> BuildSortRouteData(string sortField)
        {
            var routeData = new Dictionary<string, string>();

            var reverseSort = !string.IsNullOrEmpty(SortField)
                              && SortField.ToLower() == sortField.ToLower()
                              && !ReverseSort;
            routeData.Add(nameof(ReverseSort), reverseSort.ToString());
            routeData.Add(nameof(SortField), sortField);
            routeData.Add(nameof(SearchTerm), SearchTerm);

            return routeData;
        }

        public List<string> Employers { get; set; }

        public string SearchTerm { get; set; }

        public string SearchEmployerName
        {
            get
            {
                if (searchStrings != null)
                    return searchStrings[0].ToLower();
                return string.Empty;
            }
        }


        public string SearchAccountName
        {
            get
            {
                if (searchStrings != null && searchStrings.Length > 1)
                    return searchStrings[1].ToLower();
                return string.Empty;
            }
        }


        private string[] searchStrings 
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    return SearchTerm.Split(" - ");
                }

                return new string[] { };
            } 
        }
    }
}
