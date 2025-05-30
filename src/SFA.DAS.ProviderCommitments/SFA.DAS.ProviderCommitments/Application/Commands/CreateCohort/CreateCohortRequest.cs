﻿using System;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort
{
    public class CreateCohortRequest : IRequest<CreateCohortResponse>
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long ProviderId { get; set; }
        public Guid ReservationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string UniqueLearnerNumber { get; set; }
        public string CourseCode { get; set; }
        public int? EmploymentPrice { get; set; }
        public int? Cost { get; set; }
        public int? TrainingPrice { get; set; }
        public int? EndPointAssessmentPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OriginatorReference { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public bool IgnoreStartDateOverlap { get; set; }
        public bool? IsOnFlexiPaymentPilot { get; set; }
        public long? LearnerDataId { get; set; }
    }
}
