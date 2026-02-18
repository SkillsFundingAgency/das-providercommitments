using System;
using System.Collections.Generic;
using System.Net;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;

public class GetLearnerDetailsForProviderRequest(
    long? providerId,
    SelectLearnersRequest request)
    : IGetApiRequest
{
    public string GetUrl =>
        $"providers/{providerId}/unapproved/add/learners/select?AccountLegalEntityId={request.AccountLegalEntityId}&cohortId={request.CohortId}&SearchTerm={WebUtility.UrlEncode(request.SearchTerm)}" +
        $"&SortColumn={WebUtility.UrlEncode(request.SortColumn)}&SortDescending={request.SortDescending}&Page={request.Page}&pageSize={Constants.LearnerRecordSearch.NumberOfLearnersPerSearchPage}" +
        $"&startMonth={request.StartMonth}&startYear={request.StartYear}&courseCode={request.CourseCode}";
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
    public int FutureMonths { get; set; }
    public IEnumerable<TrainingCourse> TrainingCourses { get; set; }
}

public class GetLearnerSummary
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public long Uln { get; set; }
    public string Course { get; set; }
    public DateTime StartDate { get; set; }
}