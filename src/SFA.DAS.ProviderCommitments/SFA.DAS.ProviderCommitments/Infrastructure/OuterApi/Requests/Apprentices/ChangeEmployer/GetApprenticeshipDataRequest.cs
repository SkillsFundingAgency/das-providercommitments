using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer
{
    public class GetApprenticeshipDataRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long ApprenticeshipId { get; set; }
        public long AccountLegalEntityId { get; set; }

        public GetApprenticeshipDataRequest(long providerId, long apprenticeshipId, long accountLegalEntityId)
        {
            ProviderId = providerId;
            ApprenticeshipId = apprenticeshipId;
            AccountLegalEntityId = accountLegalEntityId;
        }

        public string GetUrl => $"provider/{ProviderId}/apprentices/{ApprenticeshipId}/change-employer/apprenticeship-data?accountLegalEntityId={AccountLegalEntityId}";
    }

    public class GetApprenticeshipDataResponse
    {
        public GetApprenticeshipResponse Apprenticeship { get; set; }
        public GetPriceEpisodesResponse PriceEpisodes { get; set; }
        public AccountLegalEntityResponse AccountLegalEntity { get; set; }
        public GetTrainingProgrammeResponse TrainingProgrammeResponse { get; set; }
    }
}
