using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Web.Services.Cache
{
    public class ChangeEmployerCacheItem
    {
        public Guid Key { get; }

        public ChangeEmployerCacheItem(Guid key)
        {
            Key = key;
        }

        public long AccountLegalEntityId { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public bool SkippedDeliveryModelSelection { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string EmploymentEndDate { get; set; }
        public int? Price { get; set; }
        public int? EmploymentPrice { get; set; }
    }
}