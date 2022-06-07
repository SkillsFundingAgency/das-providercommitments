using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Shared
{
    public class SelectEmployerFilterModel
    {
        public const string EmployerAccountLegalEntityNameConst = "EmployerAccountLegalEntityName";
        public const string EmployerAccountNameConst = "EmployerAccountName";

        public string CurrentlySortedByField { get; set; }
        public bool ReverseSort { get; set; }

        public Dictionary<string, string> BuildSortRouteData(string sortByThisField)
        {
            var routeData = new Dictionary<string, string>();

            var reverseSort = !string.IsNullOrEmpty(CurrentlySortedByField)
                              && CurrentlySortedByField.ToLower() == sortByThisField.ToLower()
                              && !ReverseSort;
            routeData.Add(nameof(ReverseSort), reverseSort.ToString());
            routeData.Add("SortField", sortByThisField);
            routeData.Add(nameof(SearchTerm), SearchTerm);

            return routeData;
        }

        public string CssClassForArrowDirection(string sortByThisField)
        {
            string sortedByHeaderClassName = "das-table__sort";

            if (!string.IsNullOrWhiteSpace(CurrentlySortedByField)
                && CurrentlySortedByField.ToLower() == sortByThisField.ToLower())
            {
                sortedByHeaderClassName += ReverseSort ? " das-table__sort--desc" : " das-table__sort--asc";
            }

            return sortedByHeaderClassName;
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
