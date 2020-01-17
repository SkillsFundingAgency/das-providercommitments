using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DetailsViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string ApprenticeName { get; set; }
        public string Employer { get; set; }
        public string Reference { get; set; }
    }
}
