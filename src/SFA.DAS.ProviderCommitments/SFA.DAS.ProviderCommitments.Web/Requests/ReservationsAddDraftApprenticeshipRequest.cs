using System;
using SFA.DAS.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class ReservationsAddDraftApprenticeshipRequest : IAuthorizationContextModel
    {
        public Guid ReservationId { get; set; }
        public string CohortPublicHashedId { get; set; }
        public long? CohortId { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
    }
}