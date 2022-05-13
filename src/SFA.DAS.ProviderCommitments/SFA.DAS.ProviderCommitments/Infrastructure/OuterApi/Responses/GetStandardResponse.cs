using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    public class GetStandardResponse
    {
        public string StandardUId { get; set; }
        public string IfateReferenceNumber { get; set; }
        public int LarsCode { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public string Version { get; set; }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }
        public List<string> Options { get; set; }
        public StandardVersionDetail VersionDetail { get; set; }

        public int Duration { get; set; }
        public int MaxFunding { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? LastDateForNewStarts { get; set; }
        public List<GetStandardFundingResponse> ApprenticeshipFunding { get; set; }
        public string StandardPageUrl { get; set; }
    }
}
