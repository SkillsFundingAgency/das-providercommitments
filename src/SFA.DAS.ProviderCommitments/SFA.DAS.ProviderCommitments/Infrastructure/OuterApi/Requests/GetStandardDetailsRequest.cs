namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetStandardDetailsRequest : IGetApiRequest
    {
        private string _courseCode { get; }
        public string GetUrl => $"TrainingCourses/standards/{_courseCode}";

        public GetStandardDetailsRequest(string courseCode)
        {
            _courseCode = courseCode;
        }
    }
}
