using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class CreateCohortWithDraftApprenticeshipRequestFromLearnerSelectedRequestMapper(
    ICacheStorageService cacheStorage,
    IOuterApiService outerApiService)
    : IMapper<Models.Cohort.LearnerSelectedRequest, CreateCohortWithDraftApprenticeshipRequest>
{
    public async Task<CreateCohortWithDraftApprenticeshipRequest> Map(Models.Cohort.LearnerSelectedRequest source)
    {
        var learner = await outerApiService.GetLearnerSelected(source.ProviderId, source.LearnerDataId);

        var cacheItem = await cacheStorage.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey);
        cacheItem.FirstName = learner.FirstName;
        cacheItem.LastName = learner.LastName;
        cacheItem.Email = learner.Email;
        cacheItem.DateOfBirth = learner.Dob;
        cacheItem.StartDate = learner.StartDate;
        cacheItem.EndDate = learner.PlannedEndDate;
        cacheItem.DeliveryModel = learner.IsFlexiJob ? DeliveryModel.FlexiJobAgency : DeliveryModel.Regular;
        cacheItem.Uln = learner.Uln.ToString();
        cacheItem.EndPointAssessmentPrice = learner.EpaoPrice;
        cacheItem.TrainingPrice = learner.TrainingPrice;
        cacheItem.CourseCode = learner.StandardCode.ToString();
        cacheItem.Cost = learner.TrainingPrice + learner.EpaoPrice;
        cacheItem.LearnerDataId = source.LearnerDataId;
        await cacheStorage.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

        return new CreateCohortWithDraftApprenticeshipRequest
        {
            CacheKey = source.CacheKey,
            ProviderId = source.ProviderId,
            ReservationId = cacheItem.ReservationId,
            EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
            AccountLegalEntityId = source.AccountLegalEntityId,
        };
    }
}
