using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.Encoding;
using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadMapperBase
    {
        private IEncodingService _encodingService;

        public FileUploadMapperBase()
        {
            _encodingService = null;
        }

        public FileUploadMapperBase(IEncodingService encodingService)
        {
            _encodingService = encodingService;
        }

        public List<BulkUploadAddDraftApprenticeshipRequest> ConvertToBulkUploadApiRequest(List<CsvRecord> csvRecords, long providerId)
        {
            return csvRecords.Select((csvRecord, index) =>
             new BulkUploadAddDraftApprenticeshipRequest()
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
                 RowNumber = (index + 1),
                 ProviderId = providerId,
                 EPAOrgId = csvRecord.EPAOrgID,
                 CohortId = GetValueOrDefault(csvRecord.CohortRef, EncodingType.CohortReference),
                 LegalEntityId = GetValueOrDefault(csvRecord.AgreementId, EncodingType.PublicAccountLegalEntityId)
             }).ToList();
        }

        private long? GetValueOrDefault(string toDecode, EncodingType encodingType)
        {
            try
            {
                return (_encodingService != null && !string.IsNullOrWhiteSpace(toDecode))
                    ? _encodingService.Decode(toDecode, encodingType) : (long?)null;
            }
            catch
            {
                return null;
            }
        }
    }
}
