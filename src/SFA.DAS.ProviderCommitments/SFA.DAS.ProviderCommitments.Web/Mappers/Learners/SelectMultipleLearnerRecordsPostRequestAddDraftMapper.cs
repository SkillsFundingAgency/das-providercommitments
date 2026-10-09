using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectMultipleLearnerRecordsPostRequestAddDraftMapper(IOuterApiService client, ICacheStorageService cacheStorage)
    : IMapper<SelectMultipleLearnerRecordsPostRequest, SelectMultipleAddDraftApprenticeshipsResult>
{
    public async Task<SelectMultipleAddDraftApprenticeshipsResult> Map(SelectMultipleLearnerRecordsPostRequest source)
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
            throw new ArgumentException("At least one selected learner is required to save draft.", nameof(source));
        }

        var apiRequest = new SelectMultipleAddDraftApprenticeshipsApimRequest();
        apiRequest.ProviderId = cacheItem.ProviderId;
        apiRequest.AccountLegalEntityId = cacheItem.AccountLegalEntityId;
        apiRequest.AgreementId = cacheItem.EmployerAccountLegalEntityPublicHashedId;
        apiRequest.LearnerIds = cacheItem.SelectedLearners.Select(x => x.Id).ToList();
        apiRequest.AccountId = cacheItem.AccountId;

        return await client.SelectMultipleAddDraftApprenticeshipsRequest(apiRequest);
    }
}