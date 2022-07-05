namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public static class BulkUploadFileRequirements
    {
        public static string[] RequiredHeaders = new[]
        {
            nameof(CsvRecord.CohortRef),
            nameof(CsvRecord.ULN),
            nameof(CsvRecord.FamilyName),
            nameof(CsvRecord.GivenNames),
            nameof(CsvRecord.DateOfBirth),
            nameof(CsvRecord.StdCode),
            nameof(CsvRecord.StartDate),
            nameof(CsvRecord.EndDate),
            nameof(CsvRecord.TotalPrice),
            nameof(CsvRecord.EPAOrgID ),
            nameof(CsvRecord.ProviderRef),
            nameof(CsvRecord.AgreementId),
            nameof(CsvRecord.EmailAddress),
        };

        //public static string[] OptionalHeaders = new[]
        //{
        //    nameof(CsvRecord.RecognisePriorLearning),
        //    nameof(CsvRecord.DurationReducedBy),
        //    nameof(CsvRecord.PriceReducedBy),
        //};

        public static int MinimumColumnCount = RequiredHeaders.Length;

        public static int MaximumColumnCount = RequiredHeaders.Length;
        //public static int MaximumColumnCount = RequiredHeaders.Length + OptionalHeaders.Length;

        public static bool CheckHeaderCount(string[] headers)
        {
            return headers.Length >= MinimumColumnCount && headers.Length <= MaximumColumnCount;
        }
    }
}