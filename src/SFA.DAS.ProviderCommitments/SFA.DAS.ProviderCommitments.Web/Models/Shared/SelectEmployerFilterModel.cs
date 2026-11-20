using SFA.DAS.ProviderCommitments.Web.Helpers;

namespace SFA.DAS.ProviderCommitments.Web.Models.Shared;

public class SelectEmployerFilterModel
{
    public const string EmployerAccountLegalEntityNameConst = "EmployerAccountLegalEntityName";
    public const string EmployerAccountNameConst = "EmployerAccountName";
    public const string AgreementId = "AgreementId";

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
    public bool IsMultiSelectJourney { get; set; }
    public Guid? CacheKey { get; set; }

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
        routeData.Add(nameof(IsMultiSelectJourney), IsMultiSelectJourney.ToString());
        routeData.Add(nameof(CacheKey), CacheKey?.ToString() ?? string.Empty);

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
            { nameof(PageNumber), pageNumber.ToString() },
            { nameof(IsMultiSelectJourney), IsMultiSelectJourney.ToString() },
            { nameof(CacheKey), CacheKey?.ToString() ?? string.Empty }
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
            { nameof(PageNumber), "1" },
            { nameof(IsMultiSelectJourney), IsMultiSelectJourney.ToString() },
            { nameof(CacheKey), CacheKey?.ToString() ?? string.Empty }
        };
    }

    public IEnumerable<PaginationPageLink> PageLinks => PaginationHelper.BuildPageLinks(PageNumber, PaginationHelper.GetTotalPages(TotalEmployersFound, PageSize), BuildPagedRouteData);

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