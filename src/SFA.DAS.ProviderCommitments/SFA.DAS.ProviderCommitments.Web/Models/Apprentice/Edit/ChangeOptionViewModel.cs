using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class ChangeOptionViewModel :IAuthorizationContextModel
    {
        public string ApprenticeshipHashedId { get; set; }
        public long ProviderId { get; set; }
        public long ApprenticeshipId { get; set; }

        public string SelectedVersion { get; set; }
        public string SelectedVersionName { get; set; }
        public string SelectedVersionUrl { get; set; }
        public IEnumerable<string> Options { get; set; }
        public string CurrentOption { get; set; }
        public string SelectedOption { get; set; }
        public bool ReturnToChangeVersion { get; set; }
        public bool ReturnToEdit { get; set; }
    }
}
