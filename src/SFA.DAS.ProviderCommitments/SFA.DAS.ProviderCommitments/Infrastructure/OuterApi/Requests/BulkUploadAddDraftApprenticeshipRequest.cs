﻿using System;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class BulkUploadAddDraftApprenticeshipRequest
    {
        public string EndDateAsString { get; set; }
        public string CostAsString { get; set; }
        public int RowNumber { get; set; }
        public string ProviderRef { get; set; }
        public string Uln { get; set; }
        public string DateOfBirthAsString { get; set; }
        public DateTime? DateOfBirth { get; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Guid? ReservationId { get; set; }
        public string OriginatorReference { get; set; }
        public string EPAOrgId { get; set; }
        public DateTime? EndDate { get; }
        public string StartDateAsString { get; set; }
        public DateTime? StartDate { get; }
        public string CourseCode { get; set; }
        public long ProviderId { get; set; }
        public string UserId { get; set; }
        public string CohortRef { get; set; }
        public long? CohortId { get; set; }
        public string AgreementId { get; set; }
        public long? LegalEntityId { get; set; }
        public int? Cost { get; }
        public long? TransferSenderId { get; set; }
    }
}
