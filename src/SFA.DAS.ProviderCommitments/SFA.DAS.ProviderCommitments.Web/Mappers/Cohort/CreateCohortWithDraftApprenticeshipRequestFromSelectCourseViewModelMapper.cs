using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class CreateCohortWithDraftApprenticeshipRequestFromSelectCourseViewModelMapper : IMapper<Models.Cohort.SelectCourseViewModel, CreateCohortWithDraftApprenticeshipRequest>
    {
        private readonly ICacheStorageService _cacheStorage;

        public CreateCohortWithDraftApprenticeshipRequestFromSelectCourseViewModelMapper(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<CreateCohortWithDraftApprenticeshipRequest> Map(Models.Cohort.SelectCourseViewModel source)
        {
            var cacheItem = await
                _cacheStorage.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey);
            cacheItem.CourseCode = source.CourseCode;
            await _cacheStorage.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

            return new CreateCohortWithDraftApprenticeshipRequest
            {
                CacheKey = source.CacheKey,
                ProviderId = source.ProviderId,
                ReservationId = source.ReservationId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                CourseCode = source.CourseCode,
                StartMonthYear = source.StartMonthYear,
                DeliveryModel = source.DeliveryModel
            };
        }
    }
}