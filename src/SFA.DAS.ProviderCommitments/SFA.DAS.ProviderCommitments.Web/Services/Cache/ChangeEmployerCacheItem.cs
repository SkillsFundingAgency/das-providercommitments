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

        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
    }
}