using System;

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
            nameof(CsvRecord.EPAOrgID),
            nameof(CsvRecord.ProviderRef),
            nameof(CsvRecord.AgreementId),
            nameof(CsvRecord.EmailAddress),
            nameof(CsvRecord.RecognisePriorLearning),
            nameof(CsvRecord.DurationReducedBy),
            nameof(CsvRecord.PriceReducedBy),
        };

        public static string[] OptionalHeaders = new[]
        {
            nameof(CsvRecord.TrainingTotalHours),
            nameof(CsvRecord.IsDurationReducedByRPL),
            nameof(CsvRecord.TrainingHoursReduction)
        };

        public static int MinimumColumnCount = RequiredHeaders.Length;

        public static int MaximumColumnCount = RequiredHeaders.Length + OptionalHeaders.Length;

        public static bool CheckHeaderCount(string[] headers)
        {
            return headers.Length >= MinimumColumnCount && headers.Length <= MaximumColumnCount;
        }

        public static bool HasMinimumRequiredColumns(string[] headers)
        {
            return headers.Contains(nameof(CsvRecord.CohortRef), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.ULN), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.FamilyName), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.GivenNames), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.DateOfBirth), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.StdCode), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.StartDate), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.EndDate), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.TotalPrice), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.EPAOrgID), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.ProviderRef), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.AgreementId), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.EmailAddress), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.RecognisePriorLearning), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.DurationReducedBy), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.PriceReducedBy), StringComparer.InvariantCultureIgnoreCase);
        }

        public static bool IsRplExtendedUpload(string[] headers)
        {
            return headers.Contains(nameof(CsvRecord.TrainingTotalHours), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.IsDurationReducedByRPL), StringComparer.InvariantCultureIgnoreCase) &&
                    headers.Contains(nameof(CsvRecord.TrainingHoursReduction), StringComparer.InvariantCultureIgnoreCase);
        }
    }
}