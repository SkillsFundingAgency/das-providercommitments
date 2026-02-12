namespace SFA.DAS.ProviderCommitments.Web.Models.Shared;

public class SelectEmployerFilterModel
{
    public const string EmployerAccountLegalEntityNameConst = "EmployerAccountLegalEntityName";
    public const string EmployerAccountNameConst = "EmployerAccountName";

    private static readonly int PageSize = Constants.SelectEmployer.NumberOfEmployersPerPage;

    public string CurrentlySortedByField { get; set; }
    public bool ReverseSort { get; set; }
    public bool UseLearnerData { get; set; }
    public long ProviderId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int TotalEmployersFound { get; set; }

    public int PagedRecordsFrom => TotalEmployersFound == 0 ? 0 : (PageNumber - 1) * PageSize + 1;
    public int PagedRecordsTo
    {
        get
        {
            var potential = PageNumber * PageSize;
            return TotalEmployersFound < potential ? TotalEmployersFound : potential;
        }
    }

    public bool ShowPageLinks => TotalEmployersFound > PageSize;

    public Dictionary<string, string> BuildSortRouteData(string sortByThisField)
    {
        var routeData = new Dictionary<string, string>();
        routeData.Add("providerId", ProviderId.ToString());
        routeData.Add(nameof(UseLearnerData), UseLearnerData.ToString());

        var reverseSort = !string.IsNullOrEmpty(CurrentlySortedByField)
                          && CurrentlySortedByField.ToLower() == sortByThisField.ToLower()
                          && !ReverseSort;
        routeData.Add(nameof(ReverseSort), reverseSort.ToString());
        routeData.Add("SortField", sortByThisField);
        routeData.Add(nameof(SearchTerm), SearchTerm ?? string.Empty);
        routeData.Add(nameof(PageNumber), "1");

        return routeData;
    }

    public Dictionary<string, string> BuildPagedRouteData(int pageNumber)
    {
        var routeData = new Dictionary<string, string>
        {
            { "providerId", ProviderId.ToString() },
            { nameof(UseLearnerData), UseLearnerData.ToString() },
            { nameof(ReverseSort), ReverseSort.ToString() },
            { "SortField", CurrentlySortedByField ?? string.Empty },
            { nameof(SearchTerm), SearchTerm ?? string.Empty },
            { nameof(PageNumber), pageNumber.ToString() }
        };
        return routeData;
    }

    public Dictionary<string, string> BuildClearSearchRouteData()
    {
        return new Dictionary<string, string>
        {
            { "providerId", ProviderId.ToString() },
            { nameof(UseLearnerData), UseLearnerData.ToString() },
            { nameof(ReverseSort), false.ToString() },
            { "SortField", string.Empty },
            { nameof(SearchTerm), string.Empty },
            { nameof(PageNumber), "1" }
        };
    }

    public IEnumerable<PageLink> PageLinks
    {
        get
        {
            var links = new List<PageLink>();
            var totalPages = (int)Math.Ceiling((double)TotalEmployersFound / PageSize);
            var totalPageLinks = totalPages < 5 ? totalPages : 5;

            if (totalPages > 1 && PageNumber > 1)
            {
                links.Add(new PageLink
                {
                    Label = "Previous",
                    AriaLabel = "Previous page",
                    RouteData = BuildPagedRouteData(PageNumber - 1)
                });
            }

            var pageNumberSeed = 1;
            if (totalPages > 5 && PageNumber > 3)
            {
                pageNumberSeed = PageNumber - 2;
                if (PageNumber > totalPages - 2)
                    pageNumberSeed = totalPages - 4;
            }

            for (var i = 0; i < totalPageLinks; i++)
            {
                links.Add(new PageLink
                {
                    Label = (pageNumberSeed + i).ToString(),
                    AriaLabel = $"Page {pageNumberSeed + i}",
                    IsCurrent = pageNumberSeed + i == PageNumber ? true : (bool?)null,
                    RouteData = BuildPagedRouteData(pageNumberSeed + i)
                });
            }

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

    public class PageLink
    {
        public string Label { get; set; }
        public string AriaLabel { get; set; }
        public bool? IsCurrent { get; set; }
        public Dictionary<string, string> RouteData { get; set; }
    }

    public string CssClassForArrowDirection(string sortByThisField)
    {
        var sortedByHeaderClassName = "das-table__sort";

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
            if (SearchStrings != null)
                return SearchStrings[0].ToLower();
            return string.Empty;
        }
    }

    public string SearchAccountName
    {
        get
        {
            if (SearchStrings != null && SearchStrings.Length > 1)
                return SearchStrings[1].ToLower();
            return string.Empty;
        }
    }

    private string[] SearchStrings 
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                return SearchTerm.Split(" - ");
            }

            return [];
        } 
    }
}