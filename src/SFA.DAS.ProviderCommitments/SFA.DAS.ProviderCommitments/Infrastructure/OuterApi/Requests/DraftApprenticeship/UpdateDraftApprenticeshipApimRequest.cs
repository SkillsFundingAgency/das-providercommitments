using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using System;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class UpdateDraftApprenticeshipApimRequest : ApimSaveDataRequest
    {
        public Party? RequestingParty => Party.Provider;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Uln { get; set; }
        public string CourseCode { get; set; }
        public string CourseOption { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
        public int? EmploymentPrice { get; set; }
        public int? Cost { get; set; }
        public int? TrainingPrice { get; set; }
        public int? EndPointAssessmentPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Reference { get; set; }
        public Guid? ReservationId { get; set; }
        public bool IgnoreStartDateOverlap { get; set; }
        public bool HasLearnerDataChanges { get; set; }
        public DateTime? LastLearnerDataSync { get; set; }
        public long? LearnerDataId {  get; set; }
    }
}
