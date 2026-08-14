using FluentValidation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectMultipleLearnerRecordsPostRequestMapper(IOuterApiService client, ICacheStorageService cacheStorage)
    : IMapper<SelectMultipleLearnerRecordsPostRequest, ValidateSelectMultipleLearnerRecordsRequest>
{
    public async Task<ValidateSelectMultipleLearnerRecordsRequest> Map(SelectMultipleLearnerRecordsPostRequest source)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(source.CacheKey.Value);

        var validationRequest = new ValidateSelectMultipleLearnerRecordsRequest()
        {
            //AccountLegalEntityId = cacheItem.AccountLegalEntityId,                                
            //ExcludeUlns = cacheItem.SelectedLearners.Select(x => x.Uln).ToList(),
            
        };      

        return validationRequest;
    }
}