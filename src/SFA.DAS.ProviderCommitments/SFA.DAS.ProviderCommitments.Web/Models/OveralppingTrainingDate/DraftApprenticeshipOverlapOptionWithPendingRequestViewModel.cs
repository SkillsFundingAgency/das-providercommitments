using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate
{
    public class DraftApprenticeshipOverlapOptionWithPendingRequestViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public OverlapOptions? OverlapOptions { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long? DraftApprenticeshipId { get; set; }
        public bool OverlappingTrainingDateRequestToggleEnabled { get; set; }
        public DateTime CreatedOn { get; set; }
        public ApprenticeshipStatus Status { get; set; }
        public bool EnableStopRequestEmail { get; set; }
    }
}
