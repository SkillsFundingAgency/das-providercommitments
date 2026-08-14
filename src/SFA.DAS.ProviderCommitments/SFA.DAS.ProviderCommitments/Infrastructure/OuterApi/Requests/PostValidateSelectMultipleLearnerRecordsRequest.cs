namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class PostValidateSelectMultipleLearnerRecordsRequest : IPostApiRequest
    {
        public string PostUrl => "BulkUpload/Validate";

        public object Data { get; set; }

        public PostValidateSelectMultipleLearnerRecordsRequest(ValidateSelectMultipleLearnerRecordsApimRequest request)
        {
            Data = request;
        }
     }
}
