using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class CsvRecordExtensions
    {
        public static List<CsvRecord> GetEmptyRecords(this List<CsvRecord> csvRecords)
        {
            return (csvRecords.Where(csvRecord => string.IsNullOrEmpty(csvRecord.CohortRef) &&
                    string.IsNullOrEmpty(csvRecord.AgreementId) &&
                    string.IsNullOrEmpty(csvRecord.ULN) &&
                    string.IsNullOrEmpty(csvRecord.FamilyName) &&
                    string.IsNullOrEmpty(csvRecord.GivenNames) &&
                    string.IsNullOrEmpty(csvRecord.DateOfBirth) &&
                    string.IsNullOrEmpty(csvRecord.EmailAddress) &&
                    string.IsNullOrEmpty(csvRecord.StdCode) &&
                    string.IsNullOrEmpty(csvRecord.StartDate) &&
                    string.IsNullOrEmpty(csvRecord.EndDate) &&
                    string.IsNullOrEmpty(csvRecord.TotalPrice) &&
                    string.IsNullOrEmpty(csvRecord.EPAOrgID) &&
                    string.IsNullOrEmpty(csvRecord.ProviderRef))
                    ).ToList();
        }
    }
}
