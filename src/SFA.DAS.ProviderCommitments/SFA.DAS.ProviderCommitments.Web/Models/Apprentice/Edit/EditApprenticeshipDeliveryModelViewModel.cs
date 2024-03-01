using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class EditApprenticeshipDeliveryModelViewModel: IDeliveryModelSelection
    {
        public List<DeliveryModel> DeliveryModels { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public string LegalEntityName { get; set; }
    }
}
