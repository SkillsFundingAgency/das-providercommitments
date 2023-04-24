using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class CreateCohortWithDraftApprenticeshipRequestFromAddDraftApprenticeshipViewModel : IMapper<AddDraftApprenticeshipViewModel, CreateCohortWithDraftApprenticeshipRequest>
    {
        private readonly ICacheStorageService _cacheStorage;

        public CreateCohortWithDraftApprenticeshipRequestFromAddDraftApprenticeshipViewModel(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<CreateCohortWithDraftApprenticeshipRequest> Map(AddDraftApprenticeshipViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<CreateCohortCacheModel>(source.CacheKey);
            cacheItem.FirstName = source.FirstName;
            cacheItem.LastName = source.LastName;
            await _cacheStorage.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

            return new CreateCohortWithDraftApprenticeshipRequest
            {
                CacheKey = source.CacheKey,
                ProviderId = source.ProviderId,
                CourseCode = source.CourseCode,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                ReservationId = source.ReservationId,
                StartMonthYear = source.StartDate.MonthYear,
                DeliveryModel = source.DeliveryModel,
                IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentPilot
            };
        }
    }
}