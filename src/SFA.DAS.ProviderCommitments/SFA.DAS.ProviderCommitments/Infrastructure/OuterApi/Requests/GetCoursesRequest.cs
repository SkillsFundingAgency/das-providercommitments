namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;

public class GetCoursesRequest(long ukprn) : IGetApiRequest
{
    public long Ukprn { get; } = ukprn;
    public string GetUrl => $"TrainingCourses/{Ukprn}/coursecodes";
}