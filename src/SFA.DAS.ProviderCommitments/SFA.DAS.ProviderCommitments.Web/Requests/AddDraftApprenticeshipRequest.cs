using System;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class AddDraftApprenticeshipRequest
    {
        public Guid ReservationId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
    }
}