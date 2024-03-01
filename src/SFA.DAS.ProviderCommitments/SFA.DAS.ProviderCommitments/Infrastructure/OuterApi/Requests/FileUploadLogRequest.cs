namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class FileUploadLogRequest : ApimSaveDataRequest
    {
        public long ProviderId { get; set; }
        public string Filename { get; set; }
        public long? RowCount { get; set; }
        public long? RplCount { get; set; }
        public string FileContent { get; set; }
    }
}
