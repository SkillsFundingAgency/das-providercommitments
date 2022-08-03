using SFA.DAS.CommitmentsV2.Shared.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class DraftApprenticeshipOverlapAlertViewModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long? DraftApprenticeshipId { get; set; }
        public bool OverlappingTrainingDateRequestToggleEnabled { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please confirm these details are correct")]
        public bool DetailsAcknowledgement { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Uln { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
