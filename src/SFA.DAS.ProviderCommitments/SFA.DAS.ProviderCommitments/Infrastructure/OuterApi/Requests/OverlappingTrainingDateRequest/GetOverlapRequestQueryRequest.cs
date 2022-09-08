namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest
{
    public class GetOverlapRequestQueryRequest : IGetApiRequest
    {
        public long ApprenticeshipId;

        public GetOverlapRequestQueryRequest(long apprenticeshipId)
        {
            ApprenticeshipId = apprenticeshipId;
        }
        public string GetUrl => $"OverlappingTrainingDateRequest/{ApprenticeshipId}/getOverlapRequest";
    }
}
