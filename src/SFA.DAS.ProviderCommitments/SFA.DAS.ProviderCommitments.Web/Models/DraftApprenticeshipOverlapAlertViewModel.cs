using System;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class DraftApprenticeshipOverlapAlertViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public string OverlapApprenticeshipHashedId { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please confirm these details are correct")]
        public bool DetailsAcknowledgement { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Uln { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid ReservationId { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }

        public string ChangeURLRoute => string.IsNullOrEmpty(this.DraftApprenticeshipHashedId)
            ? (string.IsNullOrEmpty(this.CohortReference) ? RouteNames.CohortAddApprenticeship : RouteNames.DraftApprenticeshipAddAnother)
            : RouteNames.DraftApprenticeshipEdit;
    }
}
