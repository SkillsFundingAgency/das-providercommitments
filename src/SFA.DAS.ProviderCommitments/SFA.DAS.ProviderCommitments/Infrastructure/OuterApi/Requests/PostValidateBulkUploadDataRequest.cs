namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class PostValidateBulkUploadDataRequest : IPostApiRequest
    {
        public string PostUrl => "BulkUpload/Validate";

        public object Data { get; set; }

        public bool IsRplExtended { get; set; }

        public PostValidateBulkUploadDataRequest(BulkUploadValidateApimRequest request)
        {
            Data = request;
        }
     }
}
