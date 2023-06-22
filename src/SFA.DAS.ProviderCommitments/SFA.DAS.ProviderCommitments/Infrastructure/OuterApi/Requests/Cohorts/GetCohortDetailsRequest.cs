using SFA.DAS.CommitmentsV2.Types.Dtos;
using System.Collections.Generic;
using ApprenticeshipEmployerType = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ApprenticeshipEmployerType;
using LastAction = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.LastAction;
using Party = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Party;
using TransferApprovalStatus = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.TransferApprovalStatus;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class GetCohortDetailsRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long CohortId { get; set; }

        public GetCohortDetailsRequest(long providerId, long cohortId)
        {
            ProviderId = providerId;
            CohortId = cohortId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}";
    }

    public class GetCohortDetailsResponse
    {
        public long CohortId { get; set; }
        public string CohortReference { get; set; }
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
        public long? ProviderId { get; set; }
        public bool IsFundedByTransfer => TransferSenderId.HasValue;
        public long? TransferSenderId { get; set; }
        public int? PledgeApplicationId { get; set; }
        public Party WithParty { get; set; }
        public string LatestMessageCreatedByEmployer { get; set; }
        public string LatestMessageCreatedByProvider { get; set; }
        public bool IsApprovedByEmployer { get; set; }
        public bool IsApprovedByProvider { get; set; }
        public bool IsCompleteForEmployer { get; set; }
        public bool IsCompleteForProvider { get; set; }
        public ApprenticeshipEmployerType LevyStatus { get; set; }
        public long? ChangeOfPartyRequestId { get; set; }
        public bool IsLinkedToChangeOfPartyRequest => ChangeOfPartyRequestId.HasValue;
        public TransferApprovalStatus? TransferApprovalStatus { get; set; }
        public LastAction LastAction { get; set; }
        public bool ApprenticeEmailIsRequired { get; set; }
        public bool HasNoDeclaredStandards { get; set; }
        public bool HasUnavailableFlexiJobAgencyDeliveryModel { get; set; }
        public IEnumerable<string> InvalidProviderCourseCodes { get; set; }
        public IReadOnlyCollection<DraftApprenticeshipDto> DraftApprenticeships { get; set; }
        public IEnumerable<CommitmentsV2.Types.ApprenticeshipEmailOverlap> ApprenticeshipEmailOverlaps { get; set; }
        public IEnumerable<long> RplErrorDraftApprenticeshipIds { get; set; }
    }
}
