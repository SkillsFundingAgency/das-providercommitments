using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class DeliveryModelExtension
    {
        public static string ToDescription(this DeliveryModel? deliveryModel) =>
            deliveryModel?.ToDescription();

        public static string ToDescription(this DeliveryModel deliveryModel) =>
            deliveryModel switch
            {
                DeliveryModel.PortableFlexiJob => "Portable flexi-job",
                DeliveryModel.FlexiJobAgency => "Flexi-job agency",
                _ => "Regular"
            };

        public static string ToIrregularDescription(this DeliveryModel deliveryModel) =>
            deliveryModel switch
            {
                DeliveryModel.PortableFlexiJob => "Portable flexi-job",
                DeliveryModel.FlexiJobAgency => "Flexi-job agency",
                _ => null,
            };
    }
}
