namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class PutFileUploadUpdateLogRequest : IPutApiRequest
    {
        public string PutUrl => $"BulkUpload/Logs/{LogId}/error";
        public long LogId { get; }
        public object Data { get; set; }
        public PutFileUploadUpdateLogRequest(long logId, FileUploadUpdateLogWithErrorContentRequest data)
        {
            LogId = logId;
            Data = data;
        }
    }
}
