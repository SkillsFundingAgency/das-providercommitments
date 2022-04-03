namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class PostValidateBulkUploadDataRequest : IPostApiRequest
    {
        public PostValidateBulkUploadDataRequest(BulkUploadValidateApiRequest request)
        {
            Data = request;
        }

        public string PostUrl => "BulkUpload/Validate";

        public object Data { get; set; }
    }
}
