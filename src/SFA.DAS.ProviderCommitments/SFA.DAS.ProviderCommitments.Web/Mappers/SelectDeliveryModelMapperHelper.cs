using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class SelectDeliveryModelMapperHelper : ISelectDeliveryModelMapperHelper
    {
        private readonly IApprovalsOuterApiClient _client;

        public SelectDeliveryModelMapperHelper(IApprovalsOuterApiClient client)
        {
            _client = client;
        }
        public async Task<SelectDeliveryModelViewModel> Map(long providerId, string courseCode, long? accountLegalEntityId, DeliveryModel? deliveryModel)
        {
            var response = await _client.GetProviderCourseDeliveryModels(providerId, courseCode, accountLegalEntityId ?? 0);

            return new SelectDeliveryModelViewModel
            {
                CourseCode = courseCode,
                DeliveryModel = deliveryModel,
                DeliveryModels = response.DeliveryModels.ToArray()
            };
        }
    }
}