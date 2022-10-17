namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest
{
    public class GetOverlapRequestQueryRequest : IGetApiRequest
    {
        public long DraftApprenticeshipId;

        public GetOverlapRequestQueryRequest(long draftApprenticeshipId)
        {
            DraftApprenticeshipId = draftApprenticeshipId;
        }
        public string GetUrl => $"OverlappingTrainingDateRequest/{DraftApprenticeshipId}/getOverlapRequest";
    }
}
