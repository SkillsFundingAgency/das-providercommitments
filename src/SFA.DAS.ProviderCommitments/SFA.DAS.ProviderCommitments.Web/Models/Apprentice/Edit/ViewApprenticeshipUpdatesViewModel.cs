using Newtonsoft.Json;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class ViewApprenticeshipUpdatesViewModel :
        IApprenticeshipUpdatesViewModel, IAuthorizationContextModel
    {
        public bool? UndoChanges { get; set; }
        [FromRoute]
        public long ProviderId { get; set; }
        [FromRoute]
        public string ApprenticeshipHashedId { get; set; }
        [JsonIgnore]
        public long ApprenticeshipId { get; set; }
        public string ProviderName { get; set; }
        public string EmployerName { get; set; }
        public BaseEdit OriginalApprenticeship { get; set; }
        public BaseEdit ApprenticeshipUpdates { get; set; }
    }
}
