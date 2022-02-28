using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class BulkUploadExtensions
    {
        public static BulkUploadAddDraftApprenticeshipRequest MapToBulkUploadAddDraftApprenticeshipRequest(this CsvRecord csvRecord, int rowNumber, long providerId)
        {
            return new BulkUploadAddDraftApprenticeshipRequest()
            {
                AgreementId = csvRecord.AgreementId,
                CohortRef = csvRecord.CohortRef,
                DateOfBirthAsString = csvRecord.DateOfBirth,
                Email = csvRecord.EmailAddress,
                EndDateAsString = csvRecord.EndDate,
                LastName = csvRecord.FamilyName,
                FirstName = csvRecord.GivenNames,
                StartDateAsString = csvRecord.StartDate,
                CourseCode = csvRecord.StdCode,
                CostAsString = csvRecord.TotalPrice,
                Uln = csvRecord.ULN,
                ProviderRef = csvRecord.ProviderRef,
                RowNumber = rowNumber,
                ProviderId = providerId,
                EPAOrgId = csvRecord.EPAOrgID
            };
        }
    }
}
