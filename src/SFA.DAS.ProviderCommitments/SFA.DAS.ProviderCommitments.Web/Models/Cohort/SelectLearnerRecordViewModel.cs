using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using static SFA.DAS.ProviderCommitments.Constants;
using static SFA.DAS.ProviderCommitments.Web.Models.Apprentice.ApprenticesFilterModel;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectLearnerRecordViewModel : IAuthorizationContextModel
{
    public long ProviderId { get; set; }
    public string EmployerAccountName { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public string CohortReference { get; set; }
    public long? CohortId { get; set; }
    public Guid? CacheKey { get; set; }
    public Guid? ReservationId { get; set; }

    public List<LearnerSummary> Learners { get; set; } = new();
    public string PageTitle => "Select learner from ILR";

    public string SortedByHeaderClassName { get; set; }
    public const string HeaderClassName = "das-table__sort";

    public void SortedByHeader()
    {
        SortedByHeaderClassName += HeaderClassName;
        if (FilterModel.ReverseSort)
        {
            SortedByHeaderClassName += " das-table__sort--desc";
        }
        else
        {
            SortedByHeaderClassName += " das-table__sort--asc";
        }
    }

    public DateTime? LastIlrSubmittedOn { get; set; }

    public string LastIlrSubmittedOnDesc
    {
        get
        {
            if (LastIlrSubmittedOn.HasValue == false)
            {
                return "";
            }

            var britishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            var britishDateTime = TimeZoneInfo.ConvertTimeFromUtc(LastIlrSubmittedOn.Value, britishTimeZone);

            return $"Last updated {britishDateTime.ToString("h:mmtt", System.Globalization.CultureInfo.InvariantCulture)} on {britishDateTime.ToString("dddd d MMMM", System.Globalization.CultureInfo.InvariantCulture)}";
        }
    }
    public bool ShowPageLinks => FilterModel.TotalNumberOfLearnersFound > Constants.LearnerRecordSearch.NumberOfLearnersPerSearchPage;

    public LearnerRecordsFilterModel FilterModel { get; set; }

}

public class LearnerRecordsFilterModel
{
    public long ProviderId { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public string CohortReference { get; set; }
    public int PageNumber { get; set; } = 1;
    public string SearchTerm { get; set; }
    public int TotalNumberOfLearnersFound { get; set; }
    public string TotalNumberOfApprenticeshipsFoundDescription => TotalNumberOfLearnersFound == 1
        ? "1 apprentice record"
        : $"{TotalNumberOfLearnersFound} apprentice records";

    public string SortField { get; set; }
    public bool ReverseSort { get; set; }
    public Guid? CacheKey { get; set; }
    public Guid? ReservationId { get; set; }
    public bool ShowPageLinks => TotalNumberOfLearnersFound > LearnerRecordSearch.NumberOfLearnersPerSearchPage;
    public Dictionary<string, string> RouteData => BuildRouteData();

    private const int PageSize = LearnerRecordSearch.NumberOfLearnersPerSearchPage;

    private Dictionary<string, string> BuildRouteData()
    {
        var routeData = new Dictionary<string, string>
        {
            {nameof(ProviderId), ProviderId.ToString()},
            {nameof(CacheKey), CacheKey.ToString()},
            {nameof(ReservationId), ReservationId.ToString()},
        };

        if (!string.IsNullOrWhiteSpace(EmployerAccountLegalEntityPublicHashedId))
        {
            routeData.Add(nameof(EmployerAccountLegalEntityPublicHashedId), EmployerAccountLegalEntityPublicHashedId);
        }

        if (!string.IsNullOrWhiteSpace(CohortReference))
        {
            routeData.Add(nameof(CohortReference), CohortReference);
        }

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            routeData.Add(nameof(SearchTerm), SearchTerm);
        }

        return routeData;
    }

    public IEnumerable<PageLink> PageLinks
    {
        get
        {
            var links = new List<PageLink>();
            var totalPages = (int)Math.Ceiling((double)TotalNumberOfLearnersFound / PageSize);
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
                var link = new PageLink
                {
                    Label = (pageNumberSeed + i).ToString(),
                    AriaLabel = $"Page {pageNumberSeed + i}",
                    IsCurrent = pageNumberSeed + i == PageNumber ? true : (bool?)null,
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

    private Dictionary<string, string> BuildPagedRouteData(int pageNumber)
    {
        var routeData = BuildRouteData();

        routeData.Add("page", pageNumber.ToString());

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

public class LearnerSummary
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Name => $"{FirstName} {LastName}";
    public long Uln { get; set; }
    public string CourseName { get; set; }
    public DateTime StartDate { get; set; }

    public static explicit operator LearnerSummary(GetLearnerSummary v)
    {
        return new LearnerSummary
        {
            Id = v.Id,
            FirstName = v.FirstName,
            LastName = v.LastName,
            Uln = v.Uln,
            CourseName = v.Course,
            StartDate = v.StartDate
        };
    }
}