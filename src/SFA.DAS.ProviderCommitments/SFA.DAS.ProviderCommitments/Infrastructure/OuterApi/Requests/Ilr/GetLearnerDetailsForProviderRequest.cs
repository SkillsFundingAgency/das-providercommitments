using System;
using System.Collections.Generic;
using System.Net;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;

public class GetLearnerDetailsForProviderRequest(
    long? providerId,
    long? accountLegalEntityId,
    long? cohortId,
    string searchTerm,
    string sortColumn,
    bool sortDesc,
    int page)
    : IGetApiRequest
{
    public string GetUrl =>
        $"providers/{providerId}/unapproved/add/learners/select?AccountLegalEntityId={accountLegalEntityId}&cohortId={cohortId}&SearchTerm={WebUtility.UrlEncode(searchTerm)}" +
        $"&SortColumn={WebUtility.UrlEncode(sortColumn)}&SortDescending={sortDesc}&Page={page}&pageSize={Constants.LearnerRecordSearch.NumberOfLearnersPerSearchPage}";
}

public class GetLearnerDetailsForProviderResponse
{
    public DateTime? LastSubmissionDate { get; set; }
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public long AccountLegalEntityId { get; set; }
    public string EmployerName { get; set; }
    public List<GetLearnerSummary> Learners { get; set; }
}

public class GetLearnerSummary
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public long Uln { get; set; }
    public string Course { get; set; }
}