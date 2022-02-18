namespace SFA.DAS.ProviderCommitments
{
    public static class Constants
    {
        public static class ApprenticesSearch
        {
            public const int NumberOfApprenticesPerDownloadPage = 200;
            public const int NumberOfApprenticesPerSearchPage = 100;
            public const int NumberOfApprenticesRequiredForSearch = 10;
            public const string DownloadContentType = "text/csv";
        }

        public static class BulkUpload
        {
            public const string BulkUploadErrors = "bulk-upload-errors";
        }
    }
}
