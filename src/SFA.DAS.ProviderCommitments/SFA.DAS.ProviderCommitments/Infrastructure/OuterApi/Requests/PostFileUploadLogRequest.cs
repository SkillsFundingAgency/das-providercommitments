namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class PostFileUploadLogRequest : IPostApiRequest
    {
        public string PostUrl => "BulkUpload/AddLog";

        public object Data { get; set; }
        public PostFileUploadLogRequest(FileUploadLogRequest data)
        {
            Data = data;
        }
    }
}
