using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Learners;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using CommonApprenticeshipEmployerType = SFA.DAS.Common.Domain.Types.ApprenticeshipEmployerType;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectMultipleLearnerRecordsAddToExistingCohortMapper(
    ICacheStorageService cacheStorage,
    IEncodingService encodingService,
    IOuterApiClient outerApiClient)
    : IMapper<SelectMultipleLearnerRecordsAddToExistingCohortRequest, SelectMultipleLearnerRecordsRequest>
{
    public async Task<SelectMultipleLearnerRecordsRequest> Map(SelectMultipleLearnerRecordsAddToExistingCohortRequest source)
    {
        if (!encodingService.TryDecode(source.CohortReference, EncodingType.CohortReference, out var cohortId))
        {
            throw new UnauthorizedAccessException($"Failed to decode '{RouteValueKeys.CohortReference}' value '{source.CohortReference}' using encoding type '{EncodingType.CohortReference}'");
        }
        
        var cohortDetails = await outerApiClient.Get<GetCohortDetailsResponse>(new GetCohortDetailsRequest(source.ProviderId, cohortId));
        var ulnsInCohort = cohortDetails.DraftApprenticeships
            .Select(x => x.Uln)
            .Where(uln => !string.IsNullOrWhiteSpace(uln))
            .Distinct()
            .Select(long.Parse)
            .ToList();

        var cacheItem = new SelectMultipleLearnerRecordsCacheItem(Guid.NewGuid())
        {
            ProviderId = source.ProviderId,
            CohortId = cohortId,
            CohortReference = source.CohortReference,
            EmployerAccountLegalEntityPublicHashedId = source.AccountLegalEntityHashedId,
            UseLearnerData = source.UseLearnerData,
            EmployerAccountName = cohortDetails.LegalEntityName,
            LevyStatus = cohortDetails.LevyStatus.ToString().ToEnum<CommonApprenticeshipEmployerType>(),
            AccountLegalEntityId = encodingService.Decode(source.AccountLegalEntityHashedId, EncodingType.PublicAccountLegalEntityId),
            AccountId = cohortDetails.AccountId,
            UlnsInCohort = ulnsInCohort,
            TransferSenderAccountId = cohortDetails.TransferSenderId,
            EncodedPledgeApplicationId = source.EncodedPledgeApplicationId
        };

        await cacheStorage.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);

        return new SelectMultipleLearnerRecordsRequest
        {
            ProviderId = source.ProviderId,
            CacheKey = cacheItem.CacheKey
        };
    }
}
