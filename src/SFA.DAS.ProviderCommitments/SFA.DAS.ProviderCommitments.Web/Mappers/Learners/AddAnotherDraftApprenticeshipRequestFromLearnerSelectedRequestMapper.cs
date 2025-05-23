using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Learners;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class AddAnotherDraftApprenticeshipRequestFromLearnerSelectedRequestMapper : IMapper<AddAnotherLearnerSelectedRequest, ReservationsAddDraftApprenticeshipRequest>
{
    private readonly ICacheStorageService _cacheStorage;
    private readonly IOuterApiService _outerApiService;

    public AddAnotherDraftApprenticeshipRequestFromLearnerSelectedRequestMapper(ICacheStorageService cacheStorage, IOuterApiService outerApiService)
    {
        _cacheStorage = cacheStorage;
        _outerApiService = outerApiService;
    }

    public async Task<ReservationsAddDraftApprenticeshipRequest> Map(AddAnotherLearnerSelectedRequest source)
    {
        var learner = await _outerApiService.GetLearnerSelected(source.ProviderId, source.LearnerDataId);

        var cacheItem = await _cacheStorage.RetrieveFromCache<AddAnotherApprenticeshipCacheItem>(source.CacheKey);
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
        cacheItem.Cost = learner.TrainingPrice + cacheItem.EndPointAssessmentPrice;
        cacheItem.LearnerDataId = source.LearnerDataId;
        await _cacheStorage.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

        return new ReservationsAddDraftApprenticeshipRequest
        {
            CacheKey = source.CacheKey,
            ProviderId = source.ProviderId,
            ReservationId = cacheItem.ReservationId,
            CohortReference = source.CohortReference
        };
    }
}
