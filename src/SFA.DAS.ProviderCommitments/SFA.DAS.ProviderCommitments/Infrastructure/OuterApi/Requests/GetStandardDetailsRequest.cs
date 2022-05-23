namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetStandardDetailsRequest : IGetApiRequest
    {
        private readonly string _courseCode;
        public string GetUrl => $"TrainingCourses/standards/{_courseCode}";

        public GetStandardDetailsRequest(string courseCode)
        {
            _courseCode = courseCode;
        }
    }
}
