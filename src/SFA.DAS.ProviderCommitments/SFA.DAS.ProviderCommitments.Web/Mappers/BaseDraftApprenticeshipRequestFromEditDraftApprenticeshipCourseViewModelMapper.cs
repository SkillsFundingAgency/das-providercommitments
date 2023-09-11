using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Services;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class BaseDraftApprenticeshipRequestFromEditDraftApprenticeshipCourseViewModelMapper : IMapper<EditDraftApprenticeshipCourseViewModel, BaseDraftApprenticeshipRequest>
    {
        private readonly ITempDataStorageService _tempData;

        public BaseDraftApprenticeshipRequestFromEditDraftApprenticeshipCourseViewModelMapper(ITempDataStorageService tempData)
        {
            _tempData = tempData;
        }

        public async Task<BaseDraftApprenticeshipRequest> Map(EditDraftApprenticeshipCourseViewModel source)
        {
            var draft = _tempData.RetrieveFromCache<EditDraftApprenticeshipViewModel>();
            draft.CourseCode = source.CourseCode;
            _tempData.AddToCache(draft);

            return new BaseDraftApprenticeshipRequest
            {
                CohortReference = draft.CohortReference,
                DraftApprenticeshipHashedId = draft.DraftApprenticeshipHashedId,
                ProviderId = draft.ProviderId
            };

        }
    }
}
