using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectDeliveryModelViewModelMapper : IMapper<SelectDeliveryModelRequest, SelectDeliveryModelViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;

        public SelectDeliveryModelViewModelMapper(IOuterApiClient approvalsOuterApiClient)
        {
            _outerApiClient = approvalsOuterApiClient;
        }

        public async Task<SelectDeliveryModelViewModel> Map(SelectDeliveryModelRequest source)
        {
            var apiRequest = new GetSelectDeliveryModelRequest(source.ProviderId, source.ApprenticeshipId);
            var apiResponse = await _outerApiClient.Get<GetSelectDeliveryModelResponse>(apiRequest);

            return new SelectDeliveryModelViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                LegalEntityName = apiResponse.LegalEntityName,
                DeliveryModels = apiResponse.DeliveryModels
            };
        }
    }
}
