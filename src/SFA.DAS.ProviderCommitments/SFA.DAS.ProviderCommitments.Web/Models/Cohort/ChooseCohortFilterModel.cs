using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ChooseCohortFilterModel
    {
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }

        public Dictionary<string, string> RouteData => BuildRouteData();

        private Dictionary<string, string> BuildPagedRouteData()
        {
            var routeData = BuildRouteData();
            if (!string.IsNullOrEmpty(SortField))
            {
                routeData.Add(nameof(SortField), SortField);

                routeData.Add(nameof(ReverseSort), ReverseSort.ToString());
            }

            return routeData;
        }

        public Dictionary<string, string> BuildSortRouteData(string sortField)
        {
            var routeData = BuildRouteData();

            var reverseSort = !string.IsNullOrEmpty(SortField) 
                              && SortField.ToLower() == sortField.ToLower() 
                              && !ReverseSort;
            routeData.Add(nameof(ReverseSort), reverseSort.ToString());
            routeData.Add(nameof(SortField), sortField);

            return routeData;
        }
       
    }

    
}
