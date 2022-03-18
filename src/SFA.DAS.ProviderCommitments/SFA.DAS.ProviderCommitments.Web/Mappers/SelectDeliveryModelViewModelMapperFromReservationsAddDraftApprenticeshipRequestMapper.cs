using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class SelectDeliveryModelViewModelMapperFromReservationsAddDraftApprenticeshipRequestMapper : IMapper<ReservationsAddDraftApprenticeshipRequest, SelectDeliveryModelViewModel>    {
        private readonly IApprovalsOuterApiClient _client;

        public SelectDeliveryModelViewModelMapperFromReservationsAddDraftApprenticeshipRequestMapper(IApprovalsOuterApiClient client)
        {
            _client = client;
        }

        public async Task<SelectDeliveryModelViewModel> Map(ReservationsAddDraftApprenticeshipRequest source)
        {
            var response = await _client.GetProviderCourseDeliveryModels(source.ProviderId, source.CourseCode);

            return new SelectDeliveryModelViewModel
            {
                CourseCode = source.CourseCode,
                DeliveryModel = source.DeliveryModel,
                DeliveryModels = response.DeliveryModels.ToArray()
            };
        }
    }
}