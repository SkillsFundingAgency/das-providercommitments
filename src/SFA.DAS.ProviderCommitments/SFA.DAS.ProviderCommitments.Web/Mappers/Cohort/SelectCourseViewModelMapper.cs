using System.Linq;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Features;
using SelectCourseViewModel = SFA.DAS.ProviderCommitments.Web.Models.Cohort.SelectCourseViewModel;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectCourseViewModelMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, SelectCourseViewModel>
    {
        private readonly IOuterApiClient _apiClient;
        private readonly IAuthorizationService _authorizationService;

        public SelectCourseViewModelMapper(
            IOuterApiClient apiClient,
            IAuthorizationService authorizationService)
        {
            _apiClient = apiClient;
            _authorizationService = authorizationService;
        }

        public async Task<SelectCourseViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            var apiRequest = new GetAddDraftApprenticeshipCourseRequest(source.ProviderId, source.AccountLegalEntityId);
            var apiResponse = await _apiClient.Get<GetAddDraftApprenticeshipCourseResponse>(apiRequest);

            var result = new SelectCourseViewModel
            {
                CourseCode = source.CourseCode,
                ProviderId = source.ProviderId,
                EmployerName = apiResponse.EmployerName,
                IsOnFlexiPaymentsPilot = source.IsOnFlexiPaymentPilot,
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
