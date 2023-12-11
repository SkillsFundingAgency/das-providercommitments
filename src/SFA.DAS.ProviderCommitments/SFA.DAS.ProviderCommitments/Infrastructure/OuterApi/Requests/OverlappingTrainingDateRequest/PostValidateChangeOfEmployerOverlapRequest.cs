namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest
{
    public class PostValidateChangeOfEmployerOverlapRequest : IPostApiRequest
    {
        public string PostUrl => "OverlappingTrainingDateRequest/validateChangeOfEmployerOverlap";

        public object Data { get; set; }
        public PostValidateChangeOfEmployerOverlapRequest(ValidateChangeOfEmployerOverlapApimRequest data)
        {
            Data = data;
        }
    }
}
