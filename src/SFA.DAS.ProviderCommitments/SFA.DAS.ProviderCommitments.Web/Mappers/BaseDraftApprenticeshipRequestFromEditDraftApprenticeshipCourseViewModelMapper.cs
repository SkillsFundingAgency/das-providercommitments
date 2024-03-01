using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class BaseDraftApprenticeshipRequestFromEditDraftApprenticeshipCourseViewModelMapper : IMapper<EditDraftApprenticeshipCourseViewModel, BaseDraftApprenticeshipRequest>
    {
        private readonly ITempDataStorageService _tempData;

        public BaseDraftApprenticeshipRequestFromEditDraftApprenticeshipCourseViewModelMapper(ITempDataStorageService tempData)
        {
            _tempData = tempData;
        }

        public Task<BaseDraftApprenticeshipRequest> Map(EditDraftApprenticeshipCourseViewModel source)
        {
            var draft = _tempData.RetrieveFromCache<EditDraftApprenticeshipViewModel>();
            draft.CourseCode = source.CourseCode;
            _tempData.AddToCache(draft);

            var request = new BaseDraftApprenticeshipRequest
            {
                CohortReference = draft.CohortReference,
                DraftApprenticeshipHashedId = draft.DraftApprenticeshipHashedId,
                ProviderId = draft.ProviderId
            };

            return Task.FromResult(request);
        }
    }
}
