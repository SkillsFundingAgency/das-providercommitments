using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class CreateOverlappingTrainingDateApimRequest : ApimSaveDataRequest
    {
        public long ProviderId { get; set; }
        public long DraftApprenticeshipId { get; set; }
        public long PreviousApprenticeshipId { get; set; }
    }
}
