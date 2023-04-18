using System.Linq;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SFA.DAS.ProviderCommitments.Web.Services;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Features;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship
{
    public class AddDraftApprenticeshipCourseViewModelMapper : IMapper<ReservationsAddDraftApprenticeshipRequest, AddDraftApprenticeshipCourseViewModel>
    {
        private readonly ITempDataStorageService _tempData;
        private readonly IOuterApiClient _apiClient;

        public AddDraftApprenticeshipCourseViewModelMapper(ITempDataStorageService tempData, IOuterApiClient apiClient)
        {
            _tempData = tempData;
            _apiClient = apiClient;
        }

        public async Task<AddDraftApprenticeshipCourseViewModel> Map(ReservationsAddDraftApprenticeshipRequest source)
        {
            var draft = _tempData.RetrieveFromCache<AddDraftApprenticeshipViewModel>();

            var apiRequest = new GetAddDraftApprenticeshipCourseRequest(source.ProviderId, source.CohortId.Value);
            var apiResponse = await _apiClient.Get<GetAddDraftApprenticeshipCourseResponse>(apiRequest);

            //if (!_authorizationService.IsAuthorized(ProviderFeature.FlexiblePaymentsPilot))
                //model.IsOnFlexiPaymentsPilot = false;


            var result = new AddDraftApprenticeshipCourseViewModel
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
