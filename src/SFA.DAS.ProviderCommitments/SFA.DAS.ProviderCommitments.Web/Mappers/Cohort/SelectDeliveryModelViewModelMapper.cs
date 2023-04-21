using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models;
using SelectDeliveryModelViewModel = SFA.DAS.ProviderCommitments.Web.Models.Cohort.SelectDeliveryModelViewModel;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectDeliveryModelViewModelMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, SelectDeliveryModelViewModel>
    {
        private readonly IOuterApiClient _apiClient;

        public SelectDeliveryModelViewModelMapper(IOuterApiClient outerApiClient)
        {
            _apiClient = outerApiClient;
        }

        public async Task<SelectDeliveryModelViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            var apiRequest = new GetAddDraftApprenticeshipDeliveryModelRequest(source.ProviderId, source.AccountLegalEntityId, source.CourseCode);
            var apiResponse = await _apiClient.Get<GetAddDraftApprenticeshipDeliveryModelResponse>(apiRequest);

            var result = new SelectDeliveryModelViewModel
            {
                ProviderId = source.ProviderId,
                EmployerName = apiResponse.EmployerName,
                DeliveryModels = apiResponse.DeliveryModels,
                DeliveryModel = (DeliveryModel?) source.DeliveryModel,
                IsOnFlexiPaymentsPilot = source.IsOnFlexiPaymentPilot
            };

            return result;
        }
    }
}
