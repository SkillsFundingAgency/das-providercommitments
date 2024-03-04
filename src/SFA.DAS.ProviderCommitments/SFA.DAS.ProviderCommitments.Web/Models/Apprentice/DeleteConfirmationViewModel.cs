using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DeleteConfirmationViewModel : IAuthorizationContextModel
    {   
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }        
        public bool? DeleteConfirmed { get; set; }
        public string ApprenticeshipName { get; set; }        
        public DateTime? DateOfBirth { get; set; }
        public long DraftApprenticeshipId { get; set; }
        public long CohortId { get; set; }
    }
}
