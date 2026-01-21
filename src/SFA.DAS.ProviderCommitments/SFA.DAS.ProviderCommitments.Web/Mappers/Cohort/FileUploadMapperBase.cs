using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

public class FileUploadMapperBase(IEncodingService encodingService, IOuterApiService outerApiService)
{
    private readonly Dictionary<long, long?> _transferSenderIds = new ();

    public List<BulkUploadAddDraftApprenticeshipRequest> ConvertToBulkUploadApiRequest(List<CsvRecord> csvRecords, long providerId)
    {
        return csvRecords.Select((csvRecord, index) => new BulkUploadAddDraftApprenticeshipRequest()
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
            IsDurationReducedByRPLAsString = DefaultIsDurationReducedByRplToAppropriateValueIfNotSetBasedOnDurationReducedByValue(csvRecord),
            DurationReducedByAsString = BlankDurationReducedByIfItsSetToZeroAndIsDurationReducedByRplIsNotSet(csvRecord),
            PriceReducedByAsString = csvRecord.PriceReducedBy,
        }).ToList();
    }

    private static string DefaultIsDurationReducedByRplToAppropriateValueIfNotSetBasedOnDurationReducedByValue(CsvRecord csvRecord)
    {
        if (!string.IsNullOrWhiteSpace(csvRecord.IsDurationReducedByRPL))
        {
            return csvRecord.IsDurationReducedByRPL;
        }

        if (string.IsNullOrWhiteSpace(csvRecord.RecognisePriorLearning))
        {
            return null;
        }

        var isRplTrue = string.Equals(csvRecord.RecognisePriorLearning, "true", StringComparison.OrdinalIgnoreCase);
            
        if (!isRplTrue)
        {
            return null;
        }

        var durationReducedBy = DurationReducedByValue(csvRecord.DurationReducedBy);
        if (durationReducedBy > 0)
        {
            return "TRUE";
        }

        return null;
    }

    private static string BlankDurationReducedByIfItsSetToZeroAndIsDurationReducedByRplIsNotSet(CsvRecord csvRecord)
    {
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
            var cohortInfo = await outerApiService.GetCohort(cohortId.Value);
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
            return (encodingService != null && !string.IsNullOrWhiteSpace(toDecode))
                ? encodingService.Decode(toDecode, encodingType) : (long?)null;
        }
        catch
        {
            return null;
        }
    }
}