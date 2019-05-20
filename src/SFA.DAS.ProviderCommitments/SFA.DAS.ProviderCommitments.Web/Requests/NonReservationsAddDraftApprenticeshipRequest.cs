using SFA.DAS.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class NonReservationsAddDraftApprenticeshipRequest : IAuthorizationContextModel
    {
        public long? CohortId { get; set; }

        public string CohortPublicHashedId { get; set; }
    }
}
