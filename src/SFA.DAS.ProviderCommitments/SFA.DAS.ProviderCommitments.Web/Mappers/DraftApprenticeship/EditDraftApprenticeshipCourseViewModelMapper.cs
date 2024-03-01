using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship
{
    public class EditDraftApprenticeshipCourseViewModelMapper : IMapper<DraftApprenticeshipRequest, EditDraftApprenticeshipCourseViewModel>
    {
        private readonly ITempDataStorageService _tempData;
        private readonly IOuterApiClient _apiClient;

        public EditDraftApprenticeshipCourseViewModelMapper(ITempDataStorageService tempData, IOuterApiClient apiClient)
        {
            _tempData = tempData;
            _apiClient = apiClient;
        }

        public async Task<EditDraftApprenticeshipCourseViewModel> Map(DraftApprenticeshipRequest source)
        {
            var draft = _tempData.RetrieveFromCache<EditDraftApprenticeshipViewModel>();

            var apiRequest = new GetEditDraftApprenticeshipCourseRequest(source.ProviderId, source.CohortId, source.DraftApprenticeshipId);
            var apiResponse = await _apiClient.Get<GetEditDraftApprenticeshipCourseResponse>(apiRequest);

            var result = new EditDraftApprenticeshipCourseViewModel
            {
                CourseCode = draft.CourseCode,
                ProviderId = source.ProviderId,
                EmployerName = apiResponse.EmployerName,
                ShowManagingStandardsContent = apiResponse.IsMainProvider,
                Standards = apiResponse.Standards.Select(x => new Standard { CourseCode = x.CourseCode, Name = x.Name })
            };

            return result;
        }
    }
}
