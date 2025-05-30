namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ChooseCohortFilterModel
    {
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
        public bool UseLearnerData { get; set; }

        public Dictionary<string, string> BuildSortRouteData(string sortField)
        {
            var routeData = new Dictionary<string, string> {{"UseLearnerData", UseLearnerData.ToString()}};

            var reverseSort = !string.IsNullOrEmpty(SortField) 
                              && SortField.ToLower() == sortField.ToLower() 
                              && !ReverseSort;
            routeData.Add(nameof(ReverseSort), reverseSort.ToString());
            routeData.Add(nameof(SortField), sortField);

            return routeData;
        }
       
    }

    
}
