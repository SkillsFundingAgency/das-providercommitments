using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadMapperBase
    {
        private readonly IEncodingService _encodingService;
        private readonly IOuterApiService _outerApiService;
        private readonly Dictionary<long, long?> _transferSenderIds = new ();

        public FileUploadMapperBase(IEncodingService encodingService, IOuterApiService outerApiService)
        {
            _encodingService = encodingService;
            _outerApiService = outerApiService;
        }

        public List<BulkUploadAddDraftApprenticeshipRequest> ConvertToBulkUploadApiRequest(List<CsvRecord> csvRecords, long providerId, bool extendedRpl)
        {
            return csvRecords.Select((csvRecord, index) =>
            {
                return new BulkUploadAddDraftApprenticeshipRequest()
                {
                    AgreementId = csvRecord.AgreementId,
                    CohortRef = csvRecord.CohortRef,
                    DateOfBirthAsString = csvRecord.DateOfBirth,
                    Email = csvRecord.EmailAddress,
                    EndDateAsString = csvRecord.EndDate,
                    LastName = csvRecord.FamilyName.Replace("\t", " ").Trim(),
                    FirstName = csvRecord.GivenNames.Replace("\t", " ").Trim(),
                    StartDateAsString = csvRecord.StartDate,
                    CourseCode = csvRecord.StdCode,
                    CostAsString = csvRecord.TotalPrice,
                    Uln = csvRecord.ULN,
                    ProviderRef = csvRecord.ProviderRef,
                    RowNumber = (index + 1),
                    ProviderId = providerId,
                    EPAOrgId = csvRecord.EPAOrgID,
                    CohortId = GetValueOrDefault(csvRecord.CohortRef, EncodingType.CohortReference),
                    LegalEntityId = GetValueOrDefault(csvRecord.AgreementId, EncodingType.PublicAccountLegalEntityId),
                    TransferSenderId = GetTransferSenderId(csvRecord.CohortRef).Result,
                    RecognisePriorLearningAsString = csvRecord.RecognisePriorLearning,
                    TrainingTotalHoursAsString = csvRecord.TrainingTotalHours,
                    TrainingHoursReductionAsString = csvRecord.TrainingHoursReduction,
                    IsDurationReducedByRPLAsString = DefaultIsDurationReducedByRplToAppropriateValueIfNotSetBasedOnDurationReducedByValue(csvRecord, extendedRpl),
                    DurationReducedByAsString = BlankDurationReducedByIfItsSetToZeroAndIsDurationReducedByRplIsNotSet(csvRecord, extendedRpl),
                    PriceReducedByAsString = csvRecord.PriceReducedBy,
                };

            }).ToList();
        }

        private static string DefaultIsDurationReducedByRplToAppropriateValueIfNotSetBasedOnDurationReducedByValue(CsvRecord csvRecord, bool extendedRpl)
        {
            if (!extendedRpl)
            {
                return csvRecord.IsDurationReducedByRPL;
            }

            if (!string.IsNullOrWhiteSpace(csvRecord.IsDurationReducedByRPL))
            {
                return csvRecord.IsDurationReducedByRPL;
            }

            var durationReducedBy = DurationReducedByValue(csvRecord.DurationReducedBy);
            if (durationReducedBy > 0)
            {
                return "TRUE";
            }

            if (csvRecord.DurationReducedBy == "0")
            {
                return "FALSE";
            }
            return null;
        }

        private static string BlankDurationReducedByIfItsSetToZeroAndIsDurationReducedByRplIsNotSet(CsvRecord csvRecord, bool extendedRpl)
        {
            if (!extendedRpl)
            {
                return csvRecord.DurationReducedBy;
            }

            if (!string.IsNullOrWhiteSpace(csvRecord.IsDurationReducedByRPL))
            {
                return csvRecord.DurationReducedBy;
            }

            if (csvRecord.DurationReducedBy == "0")
            {
                return null;
            }
            
            return csvRecord.DurationReducedBy;
        }

        private static long? DurationReducedByValue(string durationReducedBy)
        {
            if (long.TryParse(durationReducedBy, out var reducedBy))
            {
                return reducedBy;
            }
            return null;
        }

        private async Task<long?> GetTransferSenderId(string cohortRef)
        {
            var cohortId = GetValueOrDefault(cohortRef, EncodingType.CohortReference);

            if (cohortId.HasValue)
            {
                if (_transferSenderIds.ContainsKey(cohortId.Value))
                {
                    _transferSenderIds.TryGetValue(cohortId.Value, out long? result);
                    return result;
                }
                var cohortInfo = await _outerApiService.GetCohort(cohortId.Value);
                if (cohortInfo != null)
                {
                    _transferSenderIds.Add(cohortId.Value, cohortInfo?.TransferSenderId);
                    return cohortInfo.TransferSenderId;
                }
            }

            return null;
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