namespace SFA.DAS.ProviderCommitments.Web.Models.Shared;

public class PaginationNavViewModel
{
    public string RouteName { get; set; }
    public bool ShowPageLinks { get; set; }
    public IEnumerable<PaginationPageLink> PageLinks { get; set; } = [];

    public int? PagedRecordsFrom { get; set; }
    public int? PagedRecordsTo { get; set; }
    public int? TotalCount { get; set; }
    public string SummaryLabel { get; set; }
    public string SummarySuffix { get; set; }

    public bool ShowSummary => PagedRecordsFrom.HasValue && PagedRecordsTo.HasValue && TotalCount.HasValue && !string.IsNullOrEmpty(SummaryLabel);
}
