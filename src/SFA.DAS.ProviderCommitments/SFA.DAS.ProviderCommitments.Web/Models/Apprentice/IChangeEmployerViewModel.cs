using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public interface IChangeEmployerViewModel
    {
    }

    public class InformViewModel : IAuthorizationContextModel, IChangeEmployerViewModel
    {
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public long ProviderId { get; set; }
    }

    public class ChangeEmployerRequestDetailsViewModel : IAuthorizationContextModel, IChangeEmployerViewModel
    {
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public long ProviderId { get; set; }
    }
}