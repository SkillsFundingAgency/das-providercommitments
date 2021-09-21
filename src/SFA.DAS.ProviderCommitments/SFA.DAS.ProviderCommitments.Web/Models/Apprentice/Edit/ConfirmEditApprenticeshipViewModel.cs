namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class ConfirmEditApprenticeshipViewModel : BaseEdit
    {
        public BaseEdit OriginalApprenticeship { get; set; }
        public bool? ConfirmChanges { get; set; }

        public bool ReturnToChangeOption { get; set; }
        public bool ReturnToChangeVersion { get; set; }
    }
}
