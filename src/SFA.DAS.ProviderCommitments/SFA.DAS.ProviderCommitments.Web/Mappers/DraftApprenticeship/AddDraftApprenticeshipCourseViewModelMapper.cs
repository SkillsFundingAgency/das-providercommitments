using System.Linq;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Features;
using SelectCourseViewModel = SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship.SelectCourseViewModel;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship
{
    public class AddDraftApprenticeshipCourseViewModelMapper : IMapper<ReservationsAddDraftApprenticeshipRequest, SelectCourseViewModel>
    {
        private readonly IOuterApiClient _apiClient;
        private readonly IAuthorizationService _authorizationService;

        public AddDraftApprenticeshipCourseViewModelMapper(
            IOuterApiClient apiClient,
            IAuthorizationService authorizationService)
        {
            _apiClient = apiClient;
            _authorizationService = authorizationService;
        }

        public async Task<SelectCourseViewModel> Map(ReservationsAddDraftApprenticeshipRequest source)
        {
            var apiRequest = new GetAddDraftApprenticeshipCourseRequest(source.ProviderId, source.CohortId.Value);
            var apiResponse = await _apiClient.Get<GetAddDraftApprenticeshipCourseResponse>(apiRequest);

            var result = new SelectCourseViewModel
            {
                CourseCode = source.CourseCode,
                ProviderId = source.ProviderId,
                EmployerName = apiResponse.EmployerName,
                IsOnFlexiPaymentsPilot = source.IsOnFlexiPaymentsPilot,
                ShowManagingStandardsContent = apiResponse.IsMainProvider,
                Standards = apiResponse.Standards.Select(x => new Standard { CourseCode = x.CourseCode, Name = x.Name })
            };

            if (!await _authorizationService.IsAuthorizedAsync(ProviderFeature.FlexiblePaymentsPilot))
            {
                result.IsOnFlexiPaymentsPilot = false;
            }

            return result;
        }
    }
}
