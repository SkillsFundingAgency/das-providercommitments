using SFA.DAS.CommitmentsV2.Types;
using System.Collections.Generic;
using System;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices
{
    public class GetManageApprenticeshipDetailsRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long ApprenticeshipId { get; set; }

        public GetManageApprenticeshipDetailsRequest(long providerId, long apprenticeshipId)
        {
            ProviderId = providerId;
            ApprenticeshipId = apprenticeshipId;
        }

        public string GetUrl => $"provider/{ProviderId}/apprenticeships/{ApprenticeshipId}/details";
    }

    public class GetManageApprenticeshipDetailsResponse
    {
        public ApprenticeshipDetails Apprenticeship { get; set; }
        public IReadOnlyCollection<PriceEpisode> PriceEpisodes { get; set; }
        public IReadOnlyCollection<ApprenticeshipUpdate> ApprenticeshipUpdates { get; set; }
        public IReadOnlyCollection<DataLock> DataLocks { get; set; }
        public IReadOnlyCollection<ChangeOfPartyRequest> ChangeOfPartyRequests { get; set; }
        public IReadOnlyCollection<ChangeOfEmployerLink> ChangeOfEmployerChain { get; set; }
        public bool HasMultipleDeliveryModelOptions { get; set; }

        public class ApprenticeshipDetails
        {
            public long Id { get; set; }
            public long ProviderId { get; set; }
            public string ProviderName { get; set; }
            public long EmployerAccountId { get; set; }
            public long AccountLegalEntityId { get; set; }
            public string EmployerName { get; set; }
            public DeliveryModel DeliveryModel { get; set; }
            public string CourseCode { get; set; }
            public string CourseName { get; set; }
            public long? ContinuationOfId { get; set; }
            public long? TransferSenderId { get; set; }
            public bool HasHadDataLockSuccess { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Uln { get; set; }
            public DateTime DateOfBirth { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Version { get; set; }
            public string Option { get; set; }
            public DateTime? EmploymentEndDate { get; set; }
            public int? EmploymentPrice { get; set; }
            public long CohortId { get; set; }
            public ApprenticeshipStatus Status { get; set; }
            public ConfirmationStatus ConfirmationStatus { get; set; }
            public DateTime? StopDate { get; set; }
            public DateTime? PauseDate { get; set; }
            public DateTime? CompletionDate { get; set; }
            public DateTime? ActualStartDate { get; set; }
            public string ProviderReference { get; set; }
            public bool HasContinuation { get; set; }
            public string StandardUId { get; set; }
            public bool EmailShouldBePresent { get; set; }
            public bool EmailAddressConfirmedByApprentice { get; set; }
            public bool? RecognisePriorLearning { get; set; }
            public int? TrainingTotalHours { get; set; }
            public int? DurationReducedByHours { get; set; }
            public bool? IsDurationReducedByRpl { get; set; }
            public int? DurationReducedBy { get; set; }
            public int? PriceReducedBy { get; set; }
            public bool? IsOnFlexiPaymentPilot { get; set; }
        }

        public class PriceEpisode
        {
            public long Id { get; set; }
            public long ApprenticeshipId { get; set; }
            public decimal Cost { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime? ToDate { get; set; }
        }

        public class ApprenticeshipUpdate
        {
            public long Id { get; set; }
            public long ApprenticeshipId { get; set; }
            public Party OriginatingParty { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public Decimal? Cost { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string TrainingCode { get; set; }
            public string Version { get; set; }
            public string TrainingName { get; set; }
            public string Option { get; set; }
            public DeliveryModel? DeliveryModel { get; set; }
            public DateTime? EmploymentEndDate { get; set; }
            public int? EmploymentPrice { get; set; }
        }

        public class DataLock
        {
            public long Id { get; set; }
            public DateTime DataLockEventDatetime { get; set; }
            public string PriceEpisodeIdentifier { get; set; }
            public long ApprenticeshipId { get; set; }
            public string IlrTrainingCourseCode { get; set; }
            public DateTime? IlrActualStartDate { get; set; }
            public DateTime? IlrEffectiveFromDate { get; set; }
            public DateTime? IlrPriceEffectiveToDate { get; set; }
            public Decimal? IlrTotalCost { get; set; }
            public DataLockErrorCode ErrorCode { get; set; }
            public Status DataLockStatus { get; set; }
            public TriageStatus TriageStatus { get; set; }
            public bool IsResolved { get; set; }
        }

        public class ChangeOfPartyRequest
        {
            public long Id { get; set; }
            public ChangeOfPartyRequestType ChangeOfPartyType { get; set; }
            public Party OriginatingParty { get; set; }
            public ChangeOfPartyRequestStatus Status { get; set; }
            public string EmployerName { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int? Price { get; set; }
            public long? CohortId { get; set; }
            public Party? WithParty { get; set; }
            public long? NewApprenticeshipId { get; set; }
            public long? ProviderId { get; set; }
        }

        public class ChangeOfEmployerLink
        {
            public long ApprenticeshipId { get; set; }
            public string EmployerName { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public DateTime? StopDate { get; set; }
            public DateTime? CreatedOn { get; set; }
        }
    }
}
