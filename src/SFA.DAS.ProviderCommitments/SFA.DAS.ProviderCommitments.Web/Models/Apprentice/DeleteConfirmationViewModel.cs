using SFA.DAS.Authorization.ModelBinding;
using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DeleteConfirmationViewModel : IAuthorizationContextModel
    {   
        public long ProviderId { get; set; }
        public string CommitmentHashedId { get; set; }
        public string ApprenticeshipHashedId { get; set; }        
        public bool? DeleteConfirmed { get; set; }
        public string ApprenticeshipName { get; set; }        
        public DateTime? DateOfBirth { get; set; }
        public long ApprenticeshipId { get; set; }
        public long CommitmentId { get; set; }
    }
}
