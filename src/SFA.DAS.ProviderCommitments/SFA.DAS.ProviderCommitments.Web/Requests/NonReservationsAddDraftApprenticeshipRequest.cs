using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class NonReservationsAddDraftApprenticeshipRequest : IAuthorizationContextModel
    {
        public int ProviderId { get; set; }
        public long? CohortId { get; set; }

        public string CohortReference { get; set; }
    }
}
