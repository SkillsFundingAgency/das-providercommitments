using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Web.Services.Cache
{
    public class CreateCohortCacheModel
    {
        public CreateCohortCacheModel(Guid key)
        {
            CacheKey = key;
        }

        public Guid CacheKey { get; set; }
        public Guid ReservationId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string CourseCode { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public bool? IsOnFlexiPaymentPilot { get; set; }
    }
}
