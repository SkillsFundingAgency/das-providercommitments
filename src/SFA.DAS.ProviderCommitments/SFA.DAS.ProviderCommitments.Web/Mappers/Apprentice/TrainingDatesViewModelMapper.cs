using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class TrainingDatesViewModelMapper : IMapper<TrainingDatesRequest, TrainingDatesViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ICacheStorageService _cacheStorage;

        public TrainingDatesViewModelMapper(ICommitmentsApiClient commitmentsApiClient,
            ICacheStorageService cacheStorage)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _cacheStorage = cacheStorage;
        }

        public async Task<TrainingDatesViewModel> Map(TrainingDatesRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

            return new TrainingDatesViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ApprenticeshipStatus = apprenticeship.Status,
                StopDate = apprenticeship.StopDate,
                StartDate = new MonthYearModel(cacheItem.StartDate),
                LegalEntityName = apprenticeship.EmployerName,
                DeliveryModel = cacheItem.DeliveryModel.Value,
                CacheKey = source.CacheKey,
                SkippedDeliveryModelSelection = cacheItem.SkippedDeliveryModelSelection,
                InEditMode = source.IsEdit,
                EmploymentEndDate = new MonthYearModel(cacheItem.EmploymentEndDate),
                EndDate = new MonthYearModel(cacheItem.EndDate),
                Uln = apprenticeship.Uln
            };
        }
    }
}