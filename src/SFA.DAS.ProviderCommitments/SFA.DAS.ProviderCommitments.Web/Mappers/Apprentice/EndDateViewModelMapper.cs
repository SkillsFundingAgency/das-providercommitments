using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class EndDateViewModelMapper : IMapper<EndDateRequest, EndDateViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ICacheStorageService _cacheStorage;

        public EndDateViewModelMapper(ICommitmentsApiClient commitmentsApiClient, ICacheStorageService cacheStorage)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _cacheStorage = cacheStorage;
        }

        public async Task<EndDateViewModel> Map(EndDateRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

            return new EndDateViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                StartDate = cacheItem.StartDate,
                EmploymentEndDate = new MonthYearModel(cacheItem.EmploymentEndDate),
                EndDate = new MonthYearModel(cacheItem.EndDate),
                LegalEntityName = apprenticeship.EmployerName,
                DeliveryModel = cacheItem.DeliveryModel,
                InEditMode = source.IsEdit,
                CacheKey = source.CacheKey
            };
        }
    }
}