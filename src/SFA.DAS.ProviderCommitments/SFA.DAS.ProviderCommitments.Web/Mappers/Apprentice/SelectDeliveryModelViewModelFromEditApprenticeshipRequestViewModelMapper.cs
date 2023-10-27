using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModelMapper : IMapper<EditApprenticeshipRequestViewModel, EditApprenticeshipDeliveryModelViewModel>
    {
        private readonly IOuterApiClient _apiClient;

        public SelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModelMapper(IOuterApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<EditApprenticeshipDeliveryModelViewModel> Map(EditApprenticeshipRequestViewModel source)
        {
            var apiRequest = new GetEditApprenticeshipDeliveryModelRequest(source.ProviderId, source.ApprenticeshipId);
            var apiResponse = await _apiClient.Get<GetEditApprenticeshipDeliveryModelResponse>(apiRequest);

            return new EditApprenticeshipDeliveryModelViewModel
            {
                DeliveryModel = (DeliveryModel)source.DeliveryModel,
                DeliveryModels = apiResponse.DeliveryModels
            };
        }
    }
}