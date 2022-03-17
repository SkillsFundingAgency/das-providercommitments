using System.Linq;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectDeliveryModelViewModelMapperFromAddDraftApprenticeshipViewModel : IMapper<AddDraftApprenticeshipViewModel, SelectDeliveryModelViewModel>    {
        private readonly IApprovalsOuterApiClient _client;

        public SelectDeliveryModelViewModelMapperFromAddDraftApprenticeshipViewModel(IApprovalsOuterApiClient client)
        {
            _client = client;
        }

        public async Task<SelectDeliveryModelViewModel> Map(AddDraftApprenticeshipViewModel source)
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