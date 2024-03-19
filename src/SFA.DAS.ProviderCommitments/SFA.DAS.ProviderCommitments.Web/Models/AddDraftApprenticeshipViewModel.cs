using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class AddDraftApprenticeshipViewModel : DraftApprenticeshipViewModel, IAuthorizationContextModel
    {
        public Guid CacheKey { get; set; }
    }

    public class AddDraftApprenticeshipOrRoutePostRequest : AddDraftApprenticeshipViewModel
    {
        public string ChangeCourse { get; set; }
        public string ChangeDeliveryModel { get; set; }
        public string ChangePilotStatus { get; set; }

        public bool IsChangeCourse => ChangeCourse == "Edit";
        public bool IsChangeDeliveryModel => ChangeDeliveryModel == "Edit";
        public bool IsChangePilotStatus => ChangePilotStatus == "Edit";
        public bool IsChangeOptionSelected => IsChangeCourse || IsChangeDeliveryModel || IsChangePilotStatus;
    }
}