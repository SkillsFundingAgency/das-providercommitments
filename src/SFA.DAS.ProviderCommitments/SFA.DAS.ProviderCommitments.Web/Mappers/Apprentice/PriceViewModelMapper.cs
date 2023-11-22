using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class PriceViewModelMapper : IMapper<PriceRequest, PriceViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ICacheStorageService _cacheStorage;
        public PriceViewModelMapper(ICommitmentsApiClient commitmentsApiClient, ICacheStorageService cacheStorage)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _cacheStorage = cacheStorage;
        }

        public  async Task<PriceViewModel> Map(PriceRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

            return new PriceViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ApprenticeshipStatus = apprenticeship.Status,
                Price = cacheItem.Price,
                EmploymentPrice = cacheItem.EmploymentPrice,
                InEditMode = source.IsEdit,
                LegalEntityName = apprenticeship.EmployerName,
                DeliveryModel = cacheItem.DeliveryModel.Value,
                CacheKey = cacheItem.Key
            };
        }
    }
}
