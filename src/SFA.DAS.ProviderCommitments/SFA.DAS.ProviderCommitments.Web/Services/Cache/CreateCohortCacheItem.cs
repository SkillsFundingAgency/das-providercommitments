using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Services.Cache
{
    public class CreateCohortCacheItem : ICacheModel
    {
        public CreateCohortCacheItem(Guid key)
        {
            CacheKey = key;
        }

        public Guid CacheKey { get; set; }
        public Guid ReservationId { get; set; }
        public string StartMonthYear { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string CourseCode { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public bool? IsOnFlexiPaymentPilot { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Uln { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
