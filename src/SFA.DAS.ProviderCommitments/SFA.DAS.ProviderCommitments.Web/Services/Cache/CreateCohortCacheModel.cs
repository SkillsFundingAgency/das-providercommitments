using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Services.Cache
{
    public class CreateCohortCacheModel : ICacheModel
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

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
