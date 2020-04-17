using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class InformViewModel : IAuthorizationContextModel, IChangeEmployerInformViewModel
    {
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public long ProviderId { get; set; }
    }

    public class ChangeEmployerRequestDetailsViewModel : IAuthorizationContextModel, IChangeEmployerInformViewModel
    {
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public long ProviderId { get; set; }
    }
}