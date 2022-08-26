namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest
{
    public class PostCreateOveralappingTrainingDateRequest : IPostApiRequest
    {
        public string PostUrl => "OverlappingTrainingDateRequest/create";

        public object Data { get; set; }
        public PostCreateOveralappingTrainingDateRequest(CreateOverlappingTrainingDateApimRequest data)
        {
            Data = data;
        }
    }
}
