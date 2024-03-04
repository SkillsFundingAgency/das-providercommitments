namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class SortViewModel
    {
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

        public Dictionary<string, string> BuildSortRouteData(string sortField)
        {
            var routeData = new Dictionary<string, string>();

            var reverseSort = !string.IsNullOrEmpty(SortField)
                              && string.Equals(SortField, sortField, StringComparison.CurrentCultureIgnoreCase)
                              && !ReverseSort;

            routeData.Add(nameof(ReverseSort), reverseSort.ToString());
            routeData.Add(nameof(SortField), sortField);

            return routeData;
        }

        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
    }
}
