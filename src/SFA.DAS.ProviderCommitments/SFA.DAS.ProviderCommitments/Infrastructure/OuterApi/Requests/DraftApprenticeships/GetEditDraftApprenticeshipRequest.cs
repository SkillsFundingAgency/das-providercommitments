﻿using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships
{
    public class GetEditDraftApprenticeshipRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long CohortId { get; }
        public long DraftApprenticeshipId { get; }
        public string CourseCode { get; }

        public GetEditDraftApprenticeshipRequest(long providerId, long cohortId, long draftApprenticeshipId, string courseCode)
        {
            ProviderId = providerId;
            CohortId = cohortId;
            DraftApprenticeshipId = draftApprenticeshipId;
            CourseCode = courseCode;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/{DraftApprenticeshipId}/edit?courseCode={CourseCode}";
    }

    public class GetEditDraftApprenticeshipResponse
    {
        public Guid? ReservationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string Email { get; set; }
        public string Uln { get; set; }
        public DeliveryModel DeliveryModel { get; set; }

        public string CourseCode { get; set; }
        public string StandardUId { get; set; }
        public string CourseName { get; set; }
        public bool HasStandardOptions { get; set; }
        public string TrainingCourseOption { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? Cost { get; set; }
        public int? TrainingPrice { get; set; }
        public int? EndPointAssessmentPrice { get; set; }

        public int? EmploymentPrice { get; set; }

        public DateTime? EmploymentEndDate { get; set; }
        public string EmployerReference { get; set; }
        public string ProviderReference { get; set; }

        public long ProviderId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string ProviderName { get; set; }
        public string LegalEntityName { get; set; }

        public bool IsContinuation { get; set; }
        public bool HasMultipleDeliveryModelOptions { get; set; }
        public bool HasUnavailableDeliveryModel { get; set; }
        public bool? RecognisePriorLearning { get; set; }
        public int? DurationReducedBy { get; set; }
        public int? PriceReducedBy { get; set; }
        public bool RecognisingPriorLearningStillNeedsToBeConsidered { get; set; }
        public bool RecognisingPriorLearningExtendedStillNeedsToBeConsidered { get; set; }

        public bool? IsOnFlexiPaymentPilot { get; set; }
        public bool? EmployerHasEditedCost { get; set; }
        public bool? EmailAddressConfirmed { get; set; }
    }
}
