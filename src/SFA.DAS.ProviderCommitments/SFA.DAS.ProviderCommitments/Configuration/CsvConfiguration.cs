namespace SFA.DAS.ProviderCommitments.Configuration
{
    public class CsvConfiguration
    {
        public int MaxBulkUploadFileSize { get; set; }
        public int AllowedFileColumnCount { get; set; }
        public int MaxAllowedFileRowCount { get; set; }
    }
}