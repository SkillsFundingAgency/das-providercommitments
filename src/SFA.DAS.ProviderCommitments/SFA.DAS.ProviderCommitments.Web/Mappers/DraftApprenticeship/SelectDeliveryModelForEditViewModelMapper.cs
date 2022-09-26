using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship
{
    public class SelectDeliveryModelForEditViewModelMapper : IMapper<DraftApprenticeshipRequest, SelectDeliveryModelForEditViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ITempDataStorageService _storageService;

        public SelectDeliveryModelForEditViewModelMapper(IOuterApiClient outerApiClient, ITempDataStorageService storageService)
        {
            _outerApiClient = outerApiClient;
            _storageService = storageService;
        }

        public async Task<SelectDeliveryModelForEditViewModel> Map(DraftApprenticeshipRequest source)
        {
            var editModel = _storageService.RetrieveFromCache<EditDraftApprenticeshipViewModel>();
          
            var apiRequest = new GetEditDraftApprenticeshipSelectDeliveryModelRequest(source.ProviderId, source.CohortId, source.DraftApprenticeshipId, editModel.CourseCode);
            var apiResponse = await _outerApiClient.Get<GetEditDraftApprenticeshipSelectDeliveryModelResponse>(apiRequest);

            return new SelectDeliveryModelForEditViewModel
            {
                DeliveryModel = apiResponse.DeliveryModel,
                DeliveryModels = apiResponse.DeliveryModels,
                HasUnavailableFlexiJobAgencyDeliveryModel = apiResponse.HasUnavailableDeliveryModel && apiResponse.DeliveryModel == DeliveryModel.FlexiJobAgency,
                LegalEntityName = apiResponse.EmployerName,
                CourseCode = editModel.CourseCode,
                ShowFlexiJobAgencyDeliveryModelConfirmation = apiResponse.HasUnavailableDeliveryModel &&
                                                              apiResponse.DeliveryModel == DeliveryModel.FlexiJobAgency &&
                                                              apiResponse.DeliveryModels.Count == 1
            };
        }
    }
}
