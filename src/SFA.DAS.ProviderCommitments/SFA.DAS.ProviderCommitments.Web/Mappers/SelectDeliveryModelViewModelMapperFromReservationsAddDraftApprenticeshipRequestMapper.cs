using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class SelectDeliveryModelViewModelMapperFromReservationsAddDraftApprenticeshipRequestMapper : IMapper<ReservationsAddDraftApprenticeshipRequest, SelectDeliveryModelViewModel>
    {
        private readonly ISelectDeliveryModelMapperHelper _helper;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public SelectDeliveryModelViewModelMapperFromReservationsAddDraftApprenticeshipRequestMapper(ISelectDeliveryModelMapperHelper helper, ICommitmentsApiClient commitmentsApiClient)
        {
            _helper = helper;
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<SelectDeliveryModelViewModel> Map(ReservationsAddDraftApprenticeshipRequest source)
        {
            var cohort = await _commitmentsApiClient.GetCohort(source.CohortId.Value);

            var deliveryModelViewModel = await _helper.Map(source.ProviderId, source.CourseCode, cohort.AccountLegalEntityId, source.DeliveryModel);
            deliveryModelViewModel.ShowTrainingDetails = source.ShowTrainingDetails;
            return deliveryModelViewModel;
        }
    }
}