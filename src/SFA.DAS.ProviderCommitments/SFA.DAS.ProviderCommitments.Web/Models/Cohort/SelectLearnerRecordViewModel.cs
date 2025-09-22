using System.Globalization;
using Microsoft.AspNetCore.Html;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using static SFA.DAS.ProviderCommitments.Constants;
using static SFA.DAS.ProviderCommitments.Web.Models.Apprentice.ApprenticesFilterModel;
using System.Net;

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
    public string PageTitle => $"Select apprentices from ILR for {EmployerAccountName}";

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

            return $"Last updated {britishDateTime:h:mmtt} on {britishDateTime:dddd d MMMM}";
        }
    }
    public bool ShowPageLinks => FilterModel.TotalNumberOfLearnersFound > Constants.LearnerRecordSearch.NumberOfLearnersPerSearchPage;

    public LearnerRecordsFilterModel FilterModel { get; set; }
    public int FutureMonths { get; set; }
    public bool IsNonLevy => FutureMonths > 0;
}

public class LearnerRecordsFilterModel
{
    public long ProviderId { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public string CohortReference { get; set; }
    public int PageNumber { get; set; } = 1;
    public string SearchTerm { get; set; }
    public int TotalNumberOfLearnersFound { get; set; }
    public HtmlString TotalNumberOfApprenticeshipsFoundDescription =>
        new HtmlString($"{TotalNumberOfLearnersFound} apprentice records found " + GetFiltersUsedMessage());

    public string SortField { get; set; }
    public bool ReverseSort { get; set; }
    public Guid? CacheKey { get; set; }
    public Guid? ReservationId { get; set; }
    public bool ShowPageLinks => TotalNumberOfLearnersFound > LearnerRecordSearch.NumberOfLearnersPerSearchPage;
    public Dictionary<string, string> RouteData => BuildRouteData();
    public string StartMonth { get; set; }
    public string StartYear { get; set; } = DateTime.UtcNow.Year.ToString();
    public List<SelectListItem> MonthNames { get; set; }
    public List<SelectListItem> YearNames { get; set; }

    private const int PageSize = LearnerRecordSearch.NumberOfLearnersPerSearchPage;

    public LearnerRecordsFilterModel()
    {
        MonthNames =
        [
            new SelectListItem("All", ""),
            .. Enumerable.Range(1, 12)
                .Select(m => new SelectListItem
                {
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
                    Value = m.ToString()
                })
        ];

        YearNames = Enumerable.Range(2024, DateTime.UtcNow.Year - 2022)
                .Select(m => new SelectListItem
                {
                    Text = m.ToString(),
                    Value = m.ToString()
                }).ToList();
    }

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

        routeData.Add(nameof(StartMonth), StartMonth);
        routeData.Add(nameof(StartYear), StartYear);

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

    public string GetFiltersUsedMessage()
    {
        var filters = BuildUsedFilterList();

        if (filters.Count == 0)
        {
            return string.Empty;
        }

        var message = new StringBuilder();

        message.Append($"matching <strong>{filters[0]}</strong>");

        for (var i = 1; i < filters.Count; i++)
        {
            message.Append(i == filters.Count - 1 ? " and " : ", ");

            message.Append($"<strong>{filters[i]}</strong>");
        }

        return message.ToString();
    }

    private IList<string> BuildUsedFilterList()
    {
        var filters = new List<string>();
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            filters.Add($"‘{WebUtility.HtmlEncode(SearchTerm)}’");
        }

        if (!string.IsNullOrWhiteSpace(StartMonth))
        {
            var item = MonthNames.FirstOrDefault(x=>x.Value == StartMonth);
            if(item != null)
            {
                filters.Add(WebUtility.HtmlEncode(item.Text));
            }
        }

        filters.Add(WebUtility.HtmlEncode(StartYear));

        return filters;
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