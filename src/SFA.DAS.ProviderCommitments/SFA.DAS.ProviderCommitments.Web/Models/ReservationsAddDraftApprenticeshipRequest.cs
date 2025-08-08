using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ReservationsAddDraftApprenticeshipRequest : BaseReservationsAddDraftApprenticeshipRequest, IAuthorizationContextModel
    {
        public long? CohortId { get; set; }
    }

    public class BaseReservationsAddDraftApprenticeshipRequest
    {
        public Guid? CacheKey { get; set; }
        public string CohortReference { get; set; }
        public long ProviderId { get; set; }
        public Guid? ReservationId { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public bool? IsOnFlexiPaymentPilot { get; set; }
        public bool UseLearnerData { get; set; }

        public BaseReservationsAddDraftApprenticeshipRequest CloneBaseValues()
        {
            return this.ExplicitClone();
        }
    }
}