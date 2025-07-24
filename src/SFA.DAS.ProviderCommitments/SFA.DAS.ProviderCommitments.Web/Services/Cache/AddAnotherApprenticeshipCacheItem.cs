using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Services.Cache
{
    public class AddAnotherApprenticeshipCacheItem : ICacheModel
    {
        public AddAnotherApprenticeshipCacheItem(Guid key)
        {
            CacheKey = key;
        }

        public Guid CacheKey { get; set; }
        public Guid ReservationId { get; set; }
        public string StartMonthYear { get; set; }
        public string CohortReference { get; set; }
        public string CourseCode { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Uln { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public int? Cost { get; set; }
        public int? TrainingPrice { get; set; }
        public int? EndPointAssessmentPrice { get; set; }
        public int? EmploymentPrice { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public string Reference { get; set; }
        public long? LearnerDataId { get; set; }
    }
}
