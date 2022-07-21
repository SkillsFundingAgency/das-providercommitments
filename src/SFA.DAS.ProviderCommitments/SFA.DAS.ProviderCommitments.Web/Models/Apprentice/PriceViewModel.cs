using System;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Attributes;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class PriceViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string LegalEntityName { get; set; }
        [SuppressArgumentException(nameof(EmploymentPrice), "You must enter a valid price. For example, for £1,000 enter 1000")]
        public int? EmploymentPrice { get; set; }
        [SuppressArgumentException(nameof(Price), "You must enter a valid price. For example, for £1,000 enter 1000")]
        public int? Price { get; set; }
        public bool InEditMode { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
        public Guid CacheKey { get; set; }
    }
}
