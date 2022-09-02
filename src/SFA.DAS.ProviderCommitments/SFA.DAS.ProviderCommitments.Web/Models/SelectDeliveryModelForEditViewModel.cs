using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class SelectDeliveryModelForEditViewModel : IDeliveryModelSelection
    {
        public List<Infrastructure.OuterApi.Types.DeliveryModel> DeliveryModels { get; set; }
        public Infrastructure.OuterApi.Types.DeliveryModel? DeliveryModel { get; set; }
        public string LegalEntityName { get; set; }
    }
}