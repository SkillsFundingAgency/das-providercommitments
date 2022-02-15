namespace SFA.DAS.ProviderCommitments.Configuration
{
    public class BulkUploadFileValidationConfiguration
    {
        public int MaxBulkUploadFileSize { get; set; }
        public int AllowedFileColumnCount { get; set; }
        public int MaxAllowedFileRowCount { get; set; }
    }
}