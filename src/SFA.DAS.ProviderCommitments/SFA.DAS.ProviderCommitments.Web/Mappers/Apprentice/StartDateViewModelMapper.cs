using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class StartDateViewModelMapper : IMapper<StartDateRequest, StartDateViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ICacheStorageService _cacheStorage;

        public StartDateViewModelMapper(ICommitmentsApiClient commitmentsApiClient, ICacheStorageService cacheStorage)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _cacheStorage = cacheStorage;
        }

        public async Task<StartDateViewModel> Map(StartDateRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

            return new StartDateViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                StopDate = apprenticeship.StopDate,
                StartDate = new MonthYearModel(cacheItem.StartDate),
                LegalEntityName = apprenticeship.EmployerName,
                DeliveryModel = cacheItem.DeliveryModel.Value,
                CacheKey = source.CacheKey
            };
        }
    }
}