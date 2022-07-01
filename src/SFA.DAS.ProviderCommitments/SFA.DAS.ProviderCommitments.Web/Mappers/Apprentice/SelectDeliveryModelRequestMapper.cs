using System;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectDeliveryModelRequestMapper : IMapper<ConfirmEmployerViewModel, SelectDeliveryModelRequest>
    {
        private readonly ICacheStorageService _cacheStorage;

        public SelectDeliveryModelRequestMapper(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<SelectDeliveryModelRequest> Map(ConfirmEmployerViewModel source)
        {
            var cacheItem = new ChangeEmployerCacheItem(Guid.NewGuid())
            {
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId
            };

            await _cacheStorage.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);

            return new SelectDeliveryModelRequest
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                CacheKey = cacheItem.Key
            };
        }
    }
}
