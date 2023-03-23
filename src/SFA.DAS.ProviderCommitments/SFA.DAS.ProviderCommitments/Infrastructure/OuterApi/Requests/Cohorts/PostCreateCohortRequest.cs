namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class PostCreateCohortRequest : IPostApiRequest
    {
        public object Data { get; set; }
        
        public string PostUrl => $"cohorts";
    }
}
