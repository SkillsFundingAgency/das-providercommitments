using System;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    public class GetStandardFundingResponse
    {
        public int MaxEmployerLevyCap { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public int Duration { get; set; }
    }
}
