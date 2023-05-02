using System;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class SelectCourseViewModel : IStandardSelection
    {
        public Guid CacheKey { get; set; }
        public long ProviderId { get; set; }
        public string EmployerName { get; set; }
        public bool ShowManagingStandardsContent { get; set; }
        public bool? IsOnFlexiPaymentsPilot { get; set; }
        public string CourseCode { get; set; }
        public IEnumerable<Standard> Standards { get; set; }

        public string StartMonthYear { get; set; }
        public Guid? ReservationId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
    }
}
