using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Shared
{
    public interface IDeliveryModelSelection
    {
        List<DeliveryModel> DeliveryModels { get; set; }
        DeliveryModel? DeliveryModel { get; set; }
    }
}
