namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class PostFileUploadLogRequest : IPostApiRequest
    {
        public string PostUrl => "BulkUpload/logs";

        public object Data { get; set; }
        public PostFileUploadLogRequest(FileUploadLogRequest data)
        {
            Data = data;
        }
    }
}
