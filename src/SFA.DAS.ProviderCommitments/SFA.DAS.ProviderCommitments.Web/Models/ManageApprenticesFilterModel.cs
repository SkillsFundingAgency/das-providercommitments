using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Html;
using SFA.DAS.Commitments.Shared.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ManageApprenticesFilterModelBase
    {
        public int PageNumber { get; set; } = 1;
        public string SearchTerm { get; set; }
        public string SelectedEmployer { get; set; }
        public string SelectedCourse { get; set; }
        public string SelectedStatus { get; set; }
        public DateTime? SelectedStartDate { get; set; }
        public DateTime? SelectedEndDate { get; set; }
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
    }

    public class ManageApprenticesFilterModel : ManageApprenticesFilterModelBase
    {
        public IEnumerable<string> EmployerFilters { get; set; } = new List<string>();
        public IEnumerable<string> CourseFilters { get; set; } = new List<string>();
        public IEnumerable<string> StatusFilters { get; set; } = new List<string>();
        public IEnumerable<DateTime> StartDateFilters { get; set; } = new List<DateTime>();
        public IEnumerable<DateTime> EndDateFilters { get; set; } = new List<DateTime>();

        private const int PageSize = ProviderCommitmentsWebConstants.NumberOfApprenticesPerSearchPage;
        public int PagedRecordsFrom => TotalNumberOfApprenticeshipsFound == 0 ? 0 : (PageNumber - 1) * PageSize + 1;
        public int PagedRecordsTo {
            get
            {
                var potentialValue = PageNumber * PageSize;
                return TotalNumberOfApprenticeshipsFound < potentialValue ? TotalNumberOfApprenticeshipsFound: potentialValue;
            }
        }
        public bool ShowSearch => TotalNumberOfApprenticeships >= ProviderCommitmentsWebConstants.NumberOfApprenticesRequiredForSearch;

        public bool SearchOrFiltersApplied => !string.IsNullOrWhiteSpace(SearchTerm)
                                              || !string.IsNullOrWhiteSpace(SelectedEmployer)
                                              || !string.IsNullOrWhiteSpace(SelectedCourse)
                                              || !string.IsNullOrWhiteSpace(SelectedStatus)
                                              || SelectedStartDate.HasValue
                                              || SelectedEndDate.HasValue;

        public HtmlString FiltersUsedMessage
        {
            get
            {
                var filters = new List<string>();
                if (!string.IsNullOrWhiteSpace(SearchTerm)) filters.Add($"‘{SearchTerm}’");
                if (!string.IsNullOrWhiteSpace(SelectedEmployer)) filters.Add(SelectedEmployer);
                if (!string.IsNullOrWhiteSpace(SelectedCourse)) filters.Add(SelectedCourse);
                if (!string.IsNullOrWhiteSpace(SelectedStatus)) filters.Add(SelectedStatus);
                if (SelectedStartDate.HasValue) filters.Add(SelectedStartDate.Value.ToGdsFormatWithoutDay());
                if (SelectedEndDate.HasValue) filters.Add(SelectedEndDate.Value.ToGdsFormatWithoutDay());

                if (filters.Count == 0) return HtmlString.Empty;

                var message = new StringBuilder();

                message.Append($"matching <strong>{filters[0]}</strong>");

                for (var i = 1; i < filters.Count; i++)
                {
                    if (i == filters.Count-1)
                    {
                        message.Append(" and ");
                    }
                    else
                    {
                        message.Append(", ");
                    }

                    message.Append($"<strong>{filters[i]}</strong>");
                }

                return new HtmlString(message.ToString());
            }
        }

        public int TotalNumberOfApprenticeships { get; set; }
        public int TotalNumberOfApprenticeshipsFound { get; set; }
        public int TotalNumberOfApprenticeshipsWithAlertsFound { get; set; }
        
        public IEnumerable<PageLink> PageLinks {
            get
            {
                var links = new List<PageLink>();
                var totalPages = (int)Math.Ceiling((double)TotalNumberOfApprenticeshipsFound / PageSize);
                var totalPageLinks = totalPages < 5 ? totalPages : 5;

                //previous link
                if (totalPages > 1 && PageNumber > 1)
                {
                    links.Add(new PageLink
                    {
                        Label = "Previous",
                        AriaLabel = "Previous page",
                        RouteData = BuildPagedRouteData(PageNumber - 1)
                    });
                }

                //numbered links
                var pageNumberSeed = 1;
                if (totalPages > 5 && PageNumber > 3)
                {
                    pageNumberSeed = PageNumber - 2;

                    if (PageNumber > totalPages - 2)
                        pageNumberSeed = totalPages - 4;
                }

                for (var i = 0; i < totalPageLinks; i++)
                {
                    var link = new PageLink
                    {
                        Label = (pageNumberSeed + i).ToString(),
                        AriaLabel = $"Page {pageNumberSeed + i}",
                        IsCurrent = pageNumberSeed + i == PageNumber? true : (bool?)null,
                        RouteData = BuildPagedRouteData(pageNumberSeed + i)
                    };
                    links.Add(link);
                }

                //next link
                if (totalPages > 1 && PageNumber < totalPages)
                {
                    links.Add(new PageLink
                    {
                        Label = "Next",
                        AriaLabel = "Next page",
                        RouteData = BuildPagedRouteData(PageNumber + 1)
                    });
                }

                return links;
            }
        }

        public Dictionary<string, string> RouteData => BuildRouteData();

        private Dictionary<string, string> BuildRouteData()
        {
            var routeData = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                routeData.Add(nameof(SearchTerm), SearchTerm);
            }

            if (!string.IsNullOrWhiteSpace(SelectedEmployer))
            {
                routeData.Add(nameof(SelectedEmployer), SelectedEmployer);
            }

            if (!string.IsNullOrWhiteSpace(SelectedCourse))
            {
                routeData.Add(nameof(SelectedCourse), SelectedCourse);
            }

            if (!string.IsNullOrWhiteSpace(SelectedStatus))
            {
                routeData.Add(nameof(SelectedStatus), SelectedStatus);
            }
            
            if (SelectedStartDate.HasValue)
            {
                routeData.Add(nameof(SelectedStartDate), SelectedStartDate.Value.ToString("yyyy-MM-dd"));
            }

            if (SelectedEndDate.HasValue)
            {
                routeData.Add(nameof(SelectedEndDate), SelectedEndDate.Value.ToString("yyyy-MM-dd"));
            }

            return routeData;
        }
        
        private Dictionary<string, string> BuildPagedRouteData(int pageNumber)
        {
            var routeData = BuildRouteData();
            
            routeData.Add(nameof(PageNumber), pageNumber.ToString());

            return routeData;
        }
    }

    public class PageLink
    {
        public string Label { get; set; }
        public string AriaLabel { get; set; }
        public bool? IsCurrent { get; set; }
        public Dictionary<string, string> RouteData { get; set; }
    }
}
