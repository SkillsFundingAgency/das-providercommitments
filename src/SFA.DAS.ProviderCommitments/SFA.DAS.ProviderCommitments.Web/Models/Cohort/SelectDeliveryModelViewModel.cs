using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class SelectDeliveryModelViewModel : IAuthorizationContextModel, IDeliveryModelSelection
    {
        public Guid CacheKey { get; set; }
        public string EmployerName { get; set; }
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long CohortId { get; set; }
        public Guid? ReservationId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string CourseCode { get; set; }
        public string StartMonthYear { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public List<DeliveryModel> DeliveryModels { get; set; }
    }
}