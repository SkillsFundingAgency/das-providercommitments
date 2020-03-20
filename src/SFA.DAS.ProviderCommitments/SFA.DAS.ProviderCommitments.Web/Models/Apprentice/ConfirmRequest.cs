using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ConfirmRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string StartDate { get; set; }
        public int Price { get; set; }
    }
}
