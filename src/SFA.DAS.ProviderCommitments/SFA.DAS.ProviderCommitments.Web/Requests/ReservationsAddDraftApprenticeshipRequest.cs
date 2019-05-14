using System;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class ReservationsAddDraftApprenticeshipRequest
    {
        public Guid ReservationId { get; set; }
        public Cohort Cohort { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
    }
}