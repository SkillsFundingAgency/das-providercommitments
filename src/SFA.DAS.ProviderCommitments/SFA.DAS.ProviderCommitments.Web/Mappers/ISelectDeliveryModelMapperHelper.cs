using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public interface ISelectDeliveryModelMapperHelper
    {
        Task<SelectDeliveryModelViewModel> Map(long providerId, string courseCode, long? accountLegalEntityId, DeliveryModel? deliveryModel);
    }
}