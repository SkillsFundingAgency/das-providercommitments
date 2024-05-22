using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class TrainingDatesViewModelMapper : IMapper<TrainingDatesRequest, TrainingDatesViewModel>
    {
        private readonly ICacheStorageService _cacheStorage;
        private readonly IOuterApiClient _outerApiClient;

        public TrainingDatesViewModelMapper(ICacheStorageService cacheStorage, IOuterApiClient outerApiClient)
        {
            _cacheStorage = cacheStorage;
            _outerApiClient = outerApiClient;
        }

        public async Task<TrainingDatesViewModel> Map(TrainingDatesRequest source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

            var apiRequest = new GetManageApprenticeshipDetailsRequest(source.ProviderId, source.ApprenticeshipId);
            var apiResponse = await _outerApiClient.Get<GetManageApprenticeshipDetailsResponse>(apiRequest);

            return new TrainingDatesViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ApprenticeshipStatus = apiResponse.Apprenticeship.Status,
                StopDate = apiResponse.Apprenticeship.StopDate,
                StartDate = new MonthYearModel(cacheItem.StartDate),
                LegalEntityName = apiResponse.Apprenticeship.EmployerName,
                DeliveryModel = cacheItem.DeliveryModel.Value,
                CacheKey = source.CacheKey,
                SkippedDeliveryModelSelection = cacheItem.SkippedDeliveryModelSelection,
                InEditMode = source.IsEdit,
                EmploymentEndDate = new MonthYearModel(cacheItem.EmploymentEndDate),
                EndDate = new MonthYearModel(cacheItem.EndDate),
                CurrentStartDate = apiResponse.Apprenticeship.StartDate.Value,
                Uln = apiResponse.Apprenticeship.Uln
            };
        }
    }
}