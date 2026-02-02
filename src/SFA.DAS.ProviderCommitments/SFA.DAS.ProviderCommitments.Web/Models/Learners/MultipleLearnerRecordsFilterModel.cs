using System.Globalization;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using static SFA.DAS.ProviderCommitments.Constants;
using static SFA.DAS.ProviderCommitments.Web.Models.Apprentice.ApprenticesFilterModel;

namespace SFA.DAS.ProviderCommitments.Web.Models.Learners;
public class MultipleLearnerRecordsFilterModel
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
    public bool ShowPageLinks => TotalNumberOfLearnersFound > LearnerRecordSearch.NumberOfLearnersPerSearchPage;
    public Dictionary<string, string> RouteData => BuildRouteData();
    public string StartMonth { get; set; }
    public string StartYear { get; set; } = DateTime.UtcNow.Year.ToString();
    public List<SelectListItem> MonthNames { get; set; }
    public List<SelectListItem> YearNames { get; set; }

    private const int PageSize = LearnerRecordSearch.NumberOfLearnersPerSearchPage;

    public MultipleLearnerRecordsFilterModel()
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
        };

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
            var item = MonthNames.FirstOrDefault(x => x.Value == StartMonth);
            if (item != null)
            {
                filters.Add(WebUtility.HtmlEncode(item.Text));
            }
        }

        filters.Add(WebUtility.HtmlEncode(StartYear));

        return filters;
    }
}
