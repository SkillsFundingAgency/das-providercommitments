using System;
using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class SelectDeliveryModelViewModel : IDeliveryModelSelection
    {
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public List<DeliveryModel> DeliveryModels { get; set; }
        public string LegalEntityName { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public Guid CacheKey { get; set; }
        public bool IsEdit { get; set; }
    }
}
