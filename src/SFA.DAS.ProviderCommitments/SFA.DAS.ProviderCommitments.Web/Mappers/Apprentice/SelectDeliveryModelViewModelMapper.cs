﻿using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectDeliveryModelViewModelMapper : IMapper<SelectDeliveryModelRequest, SelectDeliveryModelViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ICacheStorageService _cacheStorage;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public SelectDeliveryModelViewModelMapper(IOuterApiClient approvalsOuterApiClient,
            ICacheStorageService cacheStorage, ICommitmentsApiClient commitmentsApiClient)
        {
            _outerApiClient = approvalsOuterApiClient;
            _cacheStorage = cacheStorage;
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<SelectDeliveryModelViewModel> Map(SelectDeliveryModelRequest source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

            var apiRequest = new GetSelectDeliveryModelRequest(source.ProviderId, source.ApprenticeshipId,
                cacheItem.AccountLegalEntityId);
            var apiResponse = await _outerApiClient.Get<GetSelectDeliveryModelResponse>(apiRequest);

            if (apiResponse.DeliveryModels.Count == 1)
            {
                cacheItem.DeliveryModel = apiResponse.DeliveryModels.Single();
                cacheItem.SkippedDeliveryModelSelection = true;
                await _cacheStorage.SaveToCache(cacheItem.Key, cacheItem, 1);
            }

            return new SelectDeliveryModelViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                LegalEntityName = apiResponse.LegalEntityName,
                DeliveryModels = apiResponse.DeliveryModels,
                DeliveryModel = cacheItem.DeliveryModel,
                CacheKey = source.CacheKey,
                IsEdit = source.IsEdit,
                ApprenticeshipStatus = apprenticeship.Status
            };
        }
    }
}