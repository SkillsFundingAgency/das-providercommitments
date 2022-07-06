using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Exceptions;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmEmployerViewModelMapper : IMapper<ConfirmEmployerRequest, ConfirmEmployerViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ICacheStorageService _cacheStorage;
        private readonly IEncodingService _encodingService;

        public ConfirmEmployerViewModelMapper(ICommitmentsApiClient commitmentsApiClient, ICacheStorageService cacheStorage, IEncodingService encodingService)
        {
            _commitmentsApiClient = commitmentsApiClient;
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

            var accountLegalEntity = await _commitmentsApiClient.GetAccountLegalEntity(accountLegalEntityId);

            return new ConfirmEmployerViewModel
            {
                EmployerAccountName = accountLegalEntity.AccountName,
                EmployerAccountLegalEntityName = accountLegalEntity.LegalEntityName,
                ProviderId = source.ProviderId,
                EmployerAccountLegalEntityPublicHashedId = _encodingService.Encode(accountLegalEntityId, EncodingType.PublicAccountLegalEntityId)
            };
        }
    }
}
