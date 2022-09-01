using SFA.DAS.CommitmentsV2.Types;
using System;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class DraftApprenticeshipOverlapAlertRequest
    {
        public string DraftApprenticeshipHashedId { get; set; }
        public long? DraftApprenticeshipId { get; set; }
        public string OverlapApprenticeshipHashedId { get; set; }
        public long? OverlapApprenticeshipId { get; set; }
        public Guid ReservationId { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    }
}