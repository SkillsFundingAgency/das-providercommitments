using System;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DetailsViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string ApprenticeName { get; set; }
        public string Employer { get; set; }
        public string Reference { get; set; }
        public ApprenticeshipStatus Status { get; set; }
        public DateTime? StopDate { get; set; }
        public string AgreementId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Uln { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProviderRef { get; set; }
        public decimal Cost { get; set; }
        public bool AllowEditApprentice { get; set; }
        public bool HasProviderPendingUpdate { get; set; }
        public bool HasEmployerPendingUpdate { get; set; }
        public DataLockSummaryStatus DataLockStatus { get; set; }

        public bool SuppressDataLockStatusReviewLink => HasEmployerPendingUpdate || HasProviderPendingUpdate;
        public TriageOption AvailableTriageOption { get; set; }
        public enum DataLockSummaryStatus
        {
            None,
            AwaitingTriage,
            HasUnresolvedDataLocks
        }

        public enum TriageOption
        {
            Restart,
            Update,
            Both
        }
    }
}
