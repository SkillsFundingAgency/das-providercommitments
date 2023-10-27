using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmEmployerViewModelMapper : IMapper<ConfirmEmployerRequest, ConfirmEmployerViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ICacheStorageService _cacheStorage;
        private readonly IEncodingService _encodingService;

        public ConfirmEmployerViewModelMapper(IOuterApiClient outerApiClient, ICacheStorageService cacheStorage, IEncodingService encodingService)
        {
            _outerApiClient = outerApiClient;
            _cacheStorage = cacheStorage;
            _encodingService = encodingService;
        }

        public async Task<ConfirmEmployerViewModel> Map(ConfirmEmployerRequest source)
        {
            long accountLegalEntityId;

            if (source.CacheKey.HasValue)
            {
                var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey.Value);
                accountLegalEntityId = cacheItem.AccountLegalEntityId;
            }
            else
            {
                accountLegalEntityId = source.AccountLegalEntityId;
            }

            var apiRequest = new GetConfirmEmployerRequest(source.ProviderId, source.ApprenticeshipId, accountLegalEntityId);
            var apiResponse = await _outerApiClient.Get<GetConfirmEmployerResponse>(apiRequest);

            return new ConfirmEmployerViewModel
            {
                LegalEntityName = apiResponse.LegalEntityName,
                EmployerAccountName = apiResponse.AccountName,
                EmployerAccountLegalEntityName = apiResponse.AccountLegalEntityName,
                ProviderId = source.ProviderId,
                EmployerAccountLegalEntityPublicHashedId = _encodingService.Encode(accountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                IsFlexiJobAgency = apiResponse.IsFlexiJobAgency,
                DeliveryModel = apiResponse.DeliveryModel
            };
        }
    }
}
