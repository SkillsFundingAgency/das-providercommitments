namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetStandardDetailsRequest : IGetApiRequest
    {
        public string _courseCode;
        public string GetUrl => $"TrainingCourses/standards/{_courseCode}";

        public GetStandardDetailsRequest(string courseCode)
        {
            _courseCode = courseCode;
        }
    }
}
