using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class ChangeVersionViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
    
        public string StandardTitle { get; set; }
        public string StandardUrl { get; set; }
        public string CurrentVersion { get; set; }
        public string SelectedVersion { get; set; }
        public IEnumerable<string> NewerVersions { get; set; }
    }
}
