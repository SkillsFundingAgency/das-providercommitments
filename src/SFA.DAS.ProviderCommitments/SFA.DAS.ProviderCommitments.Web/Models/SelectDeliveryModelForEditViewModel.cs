using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class SelectDeliveryModelForEditViewModel : IDeliveryModelSelection
    {
        public List<Infrastructure.OuterApi.Types.DeliveryModel> DeliveryModels { get; set; }
        public Infrastructure.OuterApi.Types.DeliveryModel? DeliveryModel { get; set; }
        public bool HasUnavailableFlexiJobAgencyDeliveryModel { get; set; }
        public bool ShowFlexiJobAgencyDeliveryModelConfirmation { get; set; }
        public string PageTitle => ShowFlexiJobAgencyDeliveryModelConfirmation
            ? "Confirm the apprenticeship delivery model"
            : "Select the apprenticeship delivery model";
        public string LegalEntityName { get; set; }
        public string CourseCode { get; set; }
    }
}