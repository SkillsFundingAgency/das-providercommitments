using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse
{
    public class Framework : ICourse
    {
        public int FrameworkCode { get; set; }
        public string FrameworkName { get; set; }
        public string Id { get; set; }
        public int Level { get; set; }
        public int PathwayCode { get; set; }
        public string PathwayName { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public int MaxFunding { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public IEnumerable<FundingPeriod> FundingPeriods { get; set; }
    }
}