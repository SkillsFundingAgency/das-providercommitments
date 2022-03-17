using System.Linq;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectDeliveryModelViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, SelectDeliveryModelViewModel>    {
        private readonly IApprovalsOuterApiClient _client;

        public SelectDeliveryModelViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper(IApprovalsOuterApiClient client)
        {
            _client = client;
        }

        public async Task<SelectDeliveryModelViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {

            var response = await _client.GetProviderCourseDeliveryModels(source.ProviderId, source.CourseCode);

            return new SelectDeliveryModelViewModel
            {
                //ReservationId = source.ReservationId,
                //EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                CourseCode = source.CourseCode,
                //StartMonthYear = source.StartMonthYear,
                DeliveryModel = source.DeliveryModel,
                DeliveryModels = response.DeliveryModels.ToArray()
            };
        }
    }
}