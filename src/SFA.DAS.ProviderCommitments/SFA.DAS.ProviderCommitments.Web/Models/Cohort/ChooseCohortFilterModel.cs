﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ChooseCohortFilterModel
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

            return routeData;
        }
       
    }

    
}
