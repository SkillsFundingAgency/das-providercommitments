using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class BulkUploadExtensions
    {
        public static BulkUploadAddDraftApprenticeshipRequest MapToBulkUploadAddDraftApprenticeshipRequest(this CsvRecord csvRecord, int rowNumber, long providerId, IEncodingService encodingService)
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
                EPAOrgId = csvRecord.EPAOrgID,
                CohortId = GetValueOrDefault(csvRecord.CohortRef, encodingService, EncodingType.CohortReference),
                LegalEntityId = GetValueOrDefault(csvRecord.AgreementId, encodingService, EncodingType.PublicAccountLegalEntityId)
            };
        }

        private static long? GetValueOrDefault(string toDecode, IEncodingService encodingService, EncodingType encodingType)
        {
            try
            {
                return (encodingService != null && !string.IsNullOrWhiteSpace(toDecode))
                    ? encodingService.Decode(toDecode, encodingType) : null;
            }catch
            {
                return null;
            }
        }
    }
}
