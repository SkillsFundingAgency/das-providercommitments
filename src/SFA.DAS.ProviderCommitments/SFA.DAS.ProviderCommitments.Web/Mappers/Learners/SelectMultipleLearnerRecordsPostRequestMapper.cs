using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectMultipleLearnerRecordsPostRequestMapper(IOuterApiService client, ICacheStorageService cacheStorage)
    : IMapper<SelectMultipleLearnerRecordsPostRequest, SelectMultipleLearnerRecordsViewModel>
{
    public async Task<SelectMultipleLearnerRecordsViewModel> Map(SelectMultipleLearnerRecordsPostRequest source)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(source.CacheKey.Value);

        var validationRequest = new SelectMultipleLearnerRecordsViewModel()
        {            
        };

        var apiRequest = new ValidateSelectMultipleLearnerRecordsApimRequest();
        apiRequest.ProviderId = cacheItem.ProviderId;
        apiRequest.AccountLegalEntityId = cacheItem.AccountLegalEntityId;
        apiRequest.LearnerIds = cacheItem.SelectedLearners.Select(x => x.Id).ToList();

        await client.ValidateSelectMultipleLearnerRecordsRequest(apiRequest);

        return validationRequest;
    }
}