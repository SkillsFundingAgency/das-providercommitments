using System;
using SFA.DAS.ProviderCommitments.Web.RouteValues.AccountProviders;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class AddDraftApprenticeshipRequest
    {
        public Guid ReservationId { get; set; }
        public AccountProvidersRouteValues Account { get; set; }
        public AccountLegalEntityProvidersRouteValues AccountLegalEntity { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
    }
}