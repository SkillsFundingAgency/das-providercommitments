using System.Collections.Generic;
using System.Net;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;

public class GetSelectEmployerRequest(
    long providerId,
    string searchTerm,
    string sortField,
    bool reverseSort,
    bool useLearnerData,
    int pageNumber = 1,
    int pageSize = 50)
    : IGetApiRequest
{
    private long ProviderId { get; } = providerId;
    private string SearchTerm { get; } = searchTerm;
    private string SortField { get; } = sortField;
    private bool ReverseSort { get; } = reverseSort;
    private bool UseLearnerData { get; } = useLearnerData;
    private int PageNumber { get; } = pageNumber;
    private int PageSize { get; } = pageSize;

    public string GetUrl
    {
        get
        {
            var queryParams = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                queryParams.Add($"searchTerm={WebUtility.UrlEncode(SearchTerm)}");
            }
            
            if (!string.IsNullOrWhiteSpace(SortField))
            {
                queryParams.Add($"sortField={WebUtility.UrlEncode(SortField)}");
            }
            
            queryParams.Add($"reverseSort={ReverseSort}");
            queryParams.Add($"useLearnerData={UseLearnerData}");
            queryParams.Add($"pageNumber={PageNumber}");
            queryParams.Add($"pageSize={PageSize}");

            return $"provider/{ProviderId}/unapproved/add/select-employer?{string.Join("&", queryParams)}";
        }
    }
}

public class GetSelectEmployerResponse
{
    public List<AccountProviderLegalEntityResponseItem> AccountProviderLegalEntities { get; init; } = [];
    public List<string> Employers { get; init; } = [];
    public int TotalCount { get; init; }
}

public class AccountProviderLegalEntityResponseItem
{
    public long AccountId { get; set; }
    public string AccountPublicHashedId { get; set; }
    public string AccountHashedId { get; set; }
    public string AccountName { get; set; }
    public long AccountLegalEntityId { get; set; }
    public string AccountLegalEntityPublicHashedId { get; set; }
    public string AccountLegalEntityName { get; set; }
    public long AccountProviderId { get; set; }
    public string ApprenticeshipEmployerType { get; set; }
}
