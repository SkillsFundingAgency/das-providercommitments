﻿using Microsoft.AspNetCore.Html;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ApprenticesFilterModel
    {
        public long ProviderId { get; set; }
        public int PageNumber { get; set; } = 1;
        public string SearchTerm { get; set; }
        public string SelectedEmployer { get; set; }
        public string SelectedCourse { get; set; }
        public ApprenticeshipStatus? SelectedStatus { get; set; }
        public DateTime? SelectedStartDate { get; set; }
        public DateTime? SelectedEndDate { get; set; }
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
        public Alerts? SelectedAlert { get; set; }
        public ConfirmationStatus? SelectedApprenticeConfirmation { get; set; }
        public DeliveryModel? SelectedDeliveryModel { get; set; }
        public bool? SelectedPilotStatus { get; set; }

        public IEnumerable<string> EmployerFilters { get; set; } = new List<string>();
        public IEnumerable<string> CourseFilters { get; set; } = new List<string>();
        public IEnumerable<ApprenticeshipStatus> StatusFilters { get; set; } = new List<ApprenticeshipStatus>();
        public IEnumerable<DateTime> StartDateFilters { get; set; } = new List<DateTime>();
        public IEnumerable<DateTime> EndDateFilters { get; set; } = new List<DateTime>();
        public IEnumerable<Alerts> AlertFilters { get; set; } = new List<Alerts>();
        public IEnumerable<ConfirmationStatus> ApprenticeConfirmationFilters =>
                Enum.GetValues(typeof(ConfirmationStatus)).Cast<ConfirmationStatus>().ToList();
        public IEnumerable<DeliveryModel> DeliveryModelFilters =>
                Enum.GetValues(typeof(DeliveryModel)).Cast<DeliveryModel>().ToList();

        public Dictionary<bool, string> YesNoFilters => new() { { true, "Yes"}, { false, "No"} };

        private const int PageSize = Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage;
        public int PagedRecordsFrom => TotalNumberOfApprenticeshipsFound == 0 ? 0 : (PageNumber - 1) * PageSize + 1;
        public int PagedRecordsTo {
            get
            {
                var potentialValue = PageNumber * PageSize;
                return TotalNumberOfApprenticeshipsFound < potentialValue ? TotalNumberOfApprenticeshipsFound: potentialValue;
            }
        }
        public bool ShowSearch => TotalNumberOfApprenticeships >= Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch;

        public bool SearchOrFiltersApplied => !string.IsNullOrWhiteSpace(SearchTerm)
                                              || !string.IsNullOrWhiteSpace(SelectedEmployer)
                                              || !string.IsNullOrWhiteSpace(SelectedCourse)
                                              || SelectedStatus.HasValue
                                              || SelectedStartDate.HasValue
                                              || SelectedEndDate.HasValue
                                              || SelectedAlert.HasValue
                                              || SelectedApprenticeConfirmation.HasValue
                                              || SelectedDeliveryModel.HasValue
                                              || SelectedPilotStatus.HasValue;

        public HtmlString FiltersUsedMessage => this.GetFiltersUsedMessage();

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
            var routeData = new Dictionary<string, string> { { nameof(ProviderId), ProviderId.ToString() } };

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

            if (SelectedStatus.HasValue)
            {
                routeData.Add(nameof(SelectedStatus), SelectedStatus.Value.ToString());
            }
            
            if (SelectedStartDate.HasValue)
            {
                routeData.Add(nameof(SelectedStartDate), SelectedStartDate.Value.ToString("yyyy-MM-dd"));
            }

            if (SelectedEndDate.HasValue)
            {
                routeData.Add(nameof(SelectedEndDate), SelectedEndDate.Value.ToString("yyyy-MM-dd"));
            }

            if (SelectedAlert.HasValue)
            {
                routeData.Add(nameof(SelectedAlert), SelectedAlert.Value.ToString());
            }

            if (SelectedApprenticeConfirmation.HasValue)
            {
                routeData.Add(nameof(SelectedApprenticeConfirmation), SelectedApprenticeConfirmation.Value.ToString());
            }

            if (SelectedDeliveryModel.HasValue)
            {
                routeData.Add(nameof(SelectedDeliveryModel), SelectedDeliveryModel.Value.ToString());
            }

            if (SelectedPilotStatus.HasValue)
            {
                routeData.Add(nameof(SelectedPilotStatus), SelectedPilotStatus.Value.ToString());
            }

            return routeData;
        }
        
        private Dictionary<string, string> BuildPagedRouteData(int pageNumber)
        {
            var routeData = BuildRouteData();
            
            routeData.Add(nameof(PageNumber), pageNumber.ToString());

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

        public class PageLink
        {
            public string Label { get; set; }
            public string AriaLabel { get; set; }
            public bool? IsCurrent { get; set; }
            public Dictionary<string, string> RouteData { get; set; }
        }
    }

    
}
