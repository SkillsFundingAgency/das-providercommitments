using System;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    public class GetOverlapRequestQueryResult
    {
        public long? DraftApprenticeshipId { get; set; }
        public long? PreviousApprenticeshipId { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
