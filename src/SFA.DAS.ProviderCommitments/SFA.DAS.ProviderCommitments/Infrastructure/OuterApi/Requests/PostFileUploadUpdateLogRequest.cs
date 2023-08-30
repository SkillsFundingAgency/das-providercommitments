namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class PostFileUploadUpdateLogRequest : IPostApiRequest
    {
        public string PostUrl => "BulkUpload/UpdateLog";

        public object Data { get; set; }

        public PostFileUploadUpdateLogRequest(FileUploadUpdateLogRequest data)
        {
            Data = data;
        }
    }
}
