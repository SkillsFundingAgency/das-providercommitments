using System;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class CreateCohortWithDraftApprenticeshipRequest : IAuthorizationContextModel
    {
        public Guid? ReservationId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
    }
}