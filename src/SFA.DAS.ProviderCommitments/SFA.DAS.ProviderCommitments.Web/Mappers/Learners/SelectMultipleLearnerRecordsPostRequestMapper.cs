using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectMultipleLearnerRecordsPostRequestMapper(IOuterApiService client, ICacheStorageService cacheStorage)
    : IMapper<SelectMultipleLearnerRecordsPostRequest, ValidateSelectMultipleLearnerRecordsResult>
{
    public async Task<ValidateSelectMultipleLearnerRecordsResult> Map(SelectMultipleLearnerRecordsPostRequest source)
    {
        if (source.CacheKey == null)
        {
            throw new ArgumentException("CacheKey is required", nameof(source));
        }

        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(source.CacheKey.Value);
        if (cacheItem == null)
        {
            throw new ArgumentException("Select-multiple cache entry was not found or has expired.", nameof(source));
        }

        if (cacheItem.SelectedLearners == null || !cacheItem.SelectedLearners.Any())
        {
            throw new ArgumentException("At least one selected learner is required for validate.", nameof(source));
        }

        var apiRequest = new ValidateSelectMultipleLearnerRecordsApimRequest();
        apiRequest.ProviderId = cacheItem.ProviderId;
        apiRequest.AccountLegalEntityId = cacheItem.AccountLegalEntityId;
        apiRequest.AgreementId = cacheItem.EmployerAccountLegalEntityPublicHashedId;
        apiRequest.LearnerIds = cacheItem.SelectedLearners.Select(x => x.Id).ToList();

        var result = await client.ValidateSelectMultipleLearnerRecordsRequest(apiRequest);

        if (result.ValidationErrors?.Any() == true)
        {
            cacheItem.ValidationErrors = result.ValidationErrors;
            await cacheStorage.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        return result;
    }
}
