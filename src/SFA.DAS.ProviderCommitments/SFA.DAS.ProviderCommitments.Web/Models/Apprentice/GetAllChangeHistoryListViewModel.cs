using SFA.DAS.ProviderCommitments.Enums;
using SFA.DAS.ProviderCommitments.Web.Helpers;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

public class GetAllChangeHistoryListViewModel
{
    private const int PageSize = Constants.ChangeHistory.NumberOfChangesPerPage;

    public List<GetAllChangeHistoryViewModel> ChangeHistory { get; set; } = [];
    public long ProviderId { get; set; }

    public DateTime? AvailableFrom { get; set; }
    public int PageNumber { get; set; } = 1;
    public int TotalCount { get; set; }

    public int TotalPages => PaginationHelper.GetTotalPages(TotalCount, PageSize);
    public bool IsPageOutOfRange => TotalPages > 0 && PageNumber > TotalPages;

    public int PagedRecordsFrom => TotalCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1;
    public int PagedRecordsTo => Math.Min(PageNumber * PageSize, TotalCount);
    public bool ShowPageLinks => TotalCount > PageSize;

    public IEnumerable<PaginationPageLink> PageLinks => PaginationHelper.BuildPageLinks(PageNumber, TotalPages, BuildPagedRouteData);

    private Dictionary<string, string> BuildPagedRouteData(int pageNumber) => new()
    {
        {
            nameof(PageNumber), pageNumber.ToString()
        }
    };
}

public class GetAllChangeHistoryViewModel
{
    public DateTime AppliedDate { get; set; }

    public string Description { get; set; }
    public LearningChangeType ChangeType { get; set; }
    public Guid Id { get; set; }
    public string LearnerName { get; set; }
    public string EmployerName { get; set; }
}