using System;

namespace SFA.DAS.ProviderCommitments.Models.ApiModels
{
    public class GetDraftApprenticeshipForCohortResponse
    {
        public long DraftApprenticeshipId { get; set; }
        public long CohortId { get; set; }
        public int ProviderId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public Guid? ReservationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string UniqueLearnerNumber { get; set; }
        public string CourseCode { get; set; }
        public int? Cost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OriginatorReference { get; set; }
    }
}
