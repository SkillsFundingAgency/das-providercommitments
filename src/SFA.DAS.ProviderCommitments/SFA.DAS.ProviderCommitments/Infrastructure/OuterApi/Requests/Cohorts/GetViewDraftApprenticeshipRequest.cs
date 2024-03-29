﻿using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using System;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class GetViewDraftApprenticeshipRequest : IGetApiRequest
    {
        public long ProviderId { get; set; }
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; }

        public GetViewDraftApprenticeshipRequest(long providerId, long cohortId, long draftApprenticeshipId)
        {
            ProviderId = providerId;
            CohortId = cohortId;
            DraftApprenticeshipId = draftApprenticeshipId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/{DraftApprenticeshipId}/view";
    }

    public class GetViewDraftApprenticeshipResponse
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Uln { get; set; }
        public string CourseCode { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
        public string TrainingCourseName { get; set; }
        public string TrainingCourseVersion { get; set; }
        public string TrainingCourseOption { get; set; }
        public bool TrainingCourseVersionConfirmed { get; set; }
        public string StandardUId { get; set; }
        public int? Cost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Reference { get; set; }
        public string EmployerReference { get; set; }
        public string ProviderReference { get; set; }
        public Guid? ReservationId { get; set; }
        public bool IsContinuation { get; set; }
        public long? ContinuationOfId { get; set; }
        public DateTime? OriginalStartDate { get; set; }
        public bool HasStandardOptions { get; set; }
        public int? EmploymentPrice { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public bool? RecognisePriorLearning { get; set; }
        public int? DurationReducedBy { get; set; }
        public int? PriceReducedBy { get; set; }
        public bool RecognisingPriorLearningStillNeedsToBeConsidered { get; set; }
        public bool RecognisingPriorLearningExtendedStillNeedsToBeConsidered { get; set; }
        public bool? IsOnFlexiPaymentPilot { get; set; }
        public bool? EmailAddressConfirmed { get; set; }
        public int? DurationReducedByHours { get; set; }
        public bool? IsDurationReducedByRpl { get; set; }
        public int? TrainingTotalHours { get; set; }
    }
}
