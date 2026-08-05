using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;

public class GetSelectEmployersRequest(
    long providerId,
    string searchTerm,
    string sortField,
    bool reverseSort,
    int pageNumber = 1,
    int pageSize = 50)

{
    public long ProviderId { get; } = providerId;
    public string SearchTerm { get; } = searchTerm;
    public string SortField { get; } = sortField;
    public bool ReverseSort { get; } = reverseSort;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
}

public class GetSelectEmployerResponse
{
    public List<AccountProviderLegalEntityResponseItem> AccountProviderLegalEntities { get; init; } = [];
    public List<string> Employers { get; init; } = [];
    public int TotalCount { get; init; }
    public string EmployerName { get; init; }
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