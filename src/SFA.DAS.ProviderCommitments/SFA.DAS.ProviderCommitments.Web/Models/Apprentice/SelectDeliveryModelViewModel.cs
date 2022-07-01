using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class SelectDeliveryModelViewModel
    {
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public List<DeliveryModel> DeliveryModels { get; set; }
        public string LegalEntityName { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
    }
}
