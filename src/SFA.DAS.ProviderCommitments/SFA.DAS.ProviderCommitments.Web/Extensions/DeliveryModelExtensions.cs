using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class DeliveryModelExtension
    {
        public static string ToDescription(this DeliveryModel? deliveryModel) =>
            deliveryModel?.ToDescription();

        public static string ToDescription(this DeliveryModel deliveryModel) =>
            deliveryModel switch
            {
                DeliveryModel.Flexible => "Flexi-job",
                _ => "Normal"
            };

        public static string ToAbnormalDescription(this DeliveryModel deliveryModel) =>
            deliveryModel switch
            {
                DeliveryModel.Flexible => "Flexi-job",
                _ => null,
            };
    }
}
