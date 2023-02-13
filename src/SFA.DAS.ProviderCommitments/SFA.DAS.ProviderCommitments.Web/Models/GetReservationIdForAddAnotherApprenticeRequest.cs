using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class GetReservationIdForAddAnotherApprenticeRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public long? CohortId { get; set; }
        public string AccountLegalEntityHashedId { get; set; }
        public string EncodedPledgeApplicationId { get; set; }
        public string TransferSenderHashedId { get; set; }
    }
}