using System;
using SFA.DAS.ProviderCommitments.Models;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class EditApprenticeshipRequest
    {
        public Guid ReservationId { get; set; }
        public string EmployerAccountPublicHashedId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
    }
}