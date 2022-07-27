using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class SelectDeliveryModelMapperHelper : ISelectDeliveryModelMapperHelper
    {
        private readonly IApprovalsOuterApiClient _client;
        private readonly IEncodingService _encodingService;

        public SelectDeliveryModelMapperHelper(IApprovalsOuterApiClient client, IEncodingService encodingService)
        {
            _client = client;
            _encodingService = encodingService;
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

        public async Task<bool> HasMultipleDeliveryModels(long providerId, string courseCode, string employerAccountLegalEntityPublicHashedId)
        {
            var aleId = _encodingService.Decode(employerAccountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId);
            var response = await _client.GetProviderCourseDeliveryModels(providerId, courseCode, aleId);
            return (response?.DeliveryModels.Count() > 1);
        }
    }
}