using System;
using SFA.DAS.ProviderCommitments.ModelBinding.Models;

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