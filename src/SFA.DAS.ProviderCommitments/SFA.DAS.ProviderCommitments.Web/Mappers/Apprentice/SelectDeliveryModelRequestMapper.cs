using System;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectDeliveryModelRequestMapper : IMapper<ConfirmEmployerViewModel, SelectDeliveryModelRequest>
    {
        private readonly ICacheStorageService _cacheStorage;
        private readonly IEncodingService _encodingService;

        public SelectDeliveryModelRequestMapper(ICacheStorageService cacheStorage, IEncodingService encodingService)
        {
            _cacheStorage = cacheStorage;
            _encodingService = encodingService;
        }

        public async Task<SelectDeliveryModelRequest> Map(ConfirmEmployerViewModel source)
        {
            var cacheItem = new ChangeEmployerCacheItem(Guid.NewGuid())
            {
                AccountLegalEntityId = _encodingService.Decode(source.EmployerAccountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId)
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
