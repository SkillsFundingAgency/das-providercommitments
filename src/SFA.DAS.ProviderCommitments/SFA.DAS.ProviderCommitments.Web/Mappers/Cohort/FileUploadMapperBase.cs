﻿using SFA.DAS.Encoding;
using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Linq;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadMapperBase
    {
        private IEncodingService _encodingService;
        private IOuterApiService _outerApiService;
        private Dictionary<long, long?> _transferSenderIds = new Dictionary<long, long?>();

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
                    IsDurationReducedByRPLAsString = DefaultIsDurationReducedByRplToTrueIfDurationReducedByHoldsAValueForExtendedRpl(csvRecord, extendedRpl),
                    DurationReducedByAsString = csvRecord.DurationReducedBy,
                    PriceReducedByAsString = csvRecord.PriceReducedBy,
                };

            }).ToList();
        }

        private string DefaultIsDurationReducedByRplToTrueIfDurationReducedByHoldsAValueForExtendedRpl(CsvRecord csvRecord, bool extendedRpl)
        {
            if (!extendedRpl)
            {
                return csvRecord.IsDurationReducedByRPL;
            }
            if (!string.IsNullOrWhiteSpace(csvRecord.DurationReducedBy) &&
                string.IsNullOrWhiteSpace(csvRecord.IsDurationReducedByRPL))
            {
                return "TRUE";
            }
            return csvRecord.IsDurationReducedByRPL;
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
