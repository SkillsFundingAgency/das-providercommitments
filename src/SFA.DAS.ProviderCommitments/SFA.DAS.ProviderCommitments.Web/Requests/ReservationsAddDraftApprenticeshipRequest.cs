using System;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class ReservationsAddDraftApprenticeshipRequest : IAuthorizationContextModel
    {
        public int ProviderId { get; set; }
        public Guid ReservationId { get; set; }
        public string CohortReference { get; set; }
        public long? CohortId { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
    }
}