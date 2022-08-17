using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship
{
    public class SelectDeliveryModelForEditViewModelMapper : IMapper<DraftApprenticeshipRequest, SelectDeliveryModelForEditViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;

        public SelectDeliveryModelForEditViewModelMapper(IOuterApiClient outerApiClient)
        {
            _outerApiClient = outerApiClient;
        }

        public async Task<SelectDeliveryModelForEditViewModel> Map(DraftApprenticeshipRequest source)
        {
            var apiRequest = new GetEditDraftApprenticeshipSelectDeliveryModelRequest(source.ProviderId, source.CohortId, source.DraftApprenticeshipId);
            var apiResponse = await _outerApiClient.Get<GetEditDraftApprenticeshipSelectDeliveryModelResponse>(apiRequest);

            return new SelectDeliveryModelForEditViewModel
            {
                DeliveryModel = apiResponse.DeliveryModel,
                DeliveryModels = apiResponse.DeliveryModels,
                LegalEntityName = apiResponse.EmployerName
            };
        }
    }
}
