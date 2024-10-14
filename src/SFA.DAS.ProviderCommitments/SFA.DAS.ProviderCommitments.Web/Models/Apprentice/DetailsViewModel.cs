using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Enums;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.Apprenticeships.Types;

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
        public DateTime? PauseDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string AgreementId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Uln { get; set; }
        public string CourseName { get; set; }
        public string Version { get; set; }
        public string Option { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProviderRef { get; set; }
        public decimal Cost { get; set; }
        public decimal? TrainingPrice { get; set; }
        public decimal? EndPointAssessmentPrice { get; set; }
        public bool AllowEditApprentice { get; set; }
        public bool HasProviderPendingUpdate { get; set; }
        public bool HasEmployerPendingUpdate { get; set; }
        public DataLockSummaryStatus DataLockStatus { get; set; }
        public bool HasPendingChangeOfPartyRequest { get; set; }
        public bool HasPendingOverlappingTrainingDateRequest { get; set; }
        public Party? PendingChangeOfPartyRequestWithParty { get; set; }
        public bool HasContinuation { get; set; }
        public bool ShowChangeEmployerLink => !HasContinuation && Status != ApprenticeshipStatus.Completed;

        public List<EmployerHistory> EmployerHistory { get; set; }

        public bool SuppressDataLockStatusReviewLink => HasEmployerPendingUpdate || HasProviderPendingUpdate;

        public bool ShowActionRequiredBanner =>
            HasPendingChangeOfPartyRequest && PendingChangeOfPartyRequestWithParty == Party.Provider ||
            HasEmployerPendingUpdate ||
            DataLockStatus == DetailsViewModel.DataLockSummaryStatus.HasUnresolvedDataLocks;

        public bool ShowChangesToThisApprenticeshipBanner =>
            HasPendingChangeOfPartyRequest && PendingChangeOfPartyRequestWithParty == Party.Employer ||
            HasProviderPendingUpdate ||
            DataLockStatus == DataLockSummaryStatus.AwaitingTriage;

        public TriageOption AvailableTriageOption { get; set; }
        public ConfirmationStatus? ConfirmationStatus { get; set; }
        public string Email { get; set; }
        public bool EmailShouldBePresent { get; set; }
        public bool ShowChangeVersionLink { get; set; }
        public bool HasOptions { get; set; }
        public bool SingleOption { get; set; }
        public bool EmailAddressConfirmedByApprentice { get; set; }

        public bool CanResendInvitation => !string.IsNullOrEmpty(Email) && !EmailAddressConfirmedByApprentice &&
                                           Status != ApprenticeshipStatus.Stopped;

        public DeliveryModel? DeliveryModel { get; set; }
        public int? EmploymentPrice { get; set; }
        public string EmploymentPriceDisplay => EmploymentPrice?.ToGdsCostFormat() ?? string.Empty;
        public DateTime? EmploymentEndDate { get; set; }
        public string EmploymentEndDateDisplay => EmploymentEndDate?.ToGdsFormatWithoutDay() ?? string.Empty;
        public bool? RecognisePriorLearning { get; set; }
        public int? TrainingTotalHours { get; set; }
        public string TrainingTotalHoursDisplay => $"{TrainingTotalHours} hours";
        public int? DurationReducedBy { get; set; }
        public string DurationReducedByDisplay => $"{DurationReducedBy} weeks";
        public int? DurationReducedByHours { get; set; }
        public string DurationReducedByHoursDisplay => $"{DurationReducedByHours} hours";
        public int? PriceReducedBy { get; set; }
        public bool HasMultipleDeliveryModelOptions { get; set; }
        public bool? IsOnFlexiPaymentPilot { get; set; }
        public string IsOnFlexiPaymentPilotDisplay =>
            IsOnFlexiPaymentPilot.HasValue && IsOnFlexiPaymentPilot.Value ? "Yes" : "No";
        public PaymentsStatus PaymentStatus { get; set; }
        public PendingPriceChange PendingPriceChange { get; set; }
        public PendingStartDateChange PendingStartDateChange { get; set; }
        public bool HasPendingPriceChange => PendingPriceChange != null;
        public bool HasPendingStartDateChange => PendingStartDateChange != null;
        public string ChangeOfPriceRoute => $"provider/{ProviderId}/ChangeOfPrice/{ApprenticeshipHashedId}";
        public string PendingPriceChangeRoute => $"provider/{ProviderId}/ChangeOfPrice/{ApprenticeshipHashedId}/pending";
        public string ChangeOfStartDateRoute => $"provider/{ProviderId}/ChangeOfStartDate/{ApprenticeshipHashedId}";
        public string PendingStartDateChangeRoute => $"provider/{ProviderId}/ChangeOfStartDate/{ApprenticeshipHashedId}/pending";
        public bool? CanActualStartDateBeChanged { get; set; }
        public ApprenticeDetailsBanners ShowBannersFlags { get; set; } = 0;
        public LearnerStatus LearnerStatus { get; set; }
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

    public class EmployerHistory
    {
        public string EmployerName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string HashedApprenticeshipId { get; set; }
        public bool ShowLink { get; set; }
    }

    public class PendingPriceChange
    {
        public decimal Cost { get; set; }
        public decimal? TrainingPrice { get; set; }
        public decimal? EndPointAssessmentPrice { get; set; }
        public ChangeInitiatedBy PriceChangeInitiator { get; set; }
    }

    public class PendingStartDateChange
    {
        public DateTime PendingStartDate { get; set; }
        public DateTime PendingEndDate { get; set; }
        public ChangeInitiatedBy ChangeInitiatedBy { get; set; }
    }

    public class PaymentsStatus
    {
        public string Status { get; set; }
        public bool PaymentsFrozen { get; set; }
        public string ReasonFrozen { get; set; }
        public DateTime? FrozenOn { get; set; }
    }
}