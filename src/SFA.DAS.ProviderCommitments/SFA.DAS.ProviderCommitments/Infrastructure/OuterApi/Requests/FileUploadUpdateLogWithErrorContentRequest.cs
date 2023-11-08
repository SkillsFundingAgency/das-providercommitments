namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class FileUploadUpdateLogWithErrorContentRequest : ApimSaveDataRequest
    {
        public long ProviderId { get; set; }
        public string ErrorContent { get; set; }
    }
}