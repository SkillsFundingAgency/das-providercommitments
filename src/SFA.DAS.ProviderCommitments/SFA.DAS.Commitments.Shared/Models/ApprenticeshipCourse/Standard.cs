using System;
using System.Collections.Generic;

namespace SFA.DAS.Commitments.Shared.Models.ApprenticeshipCourse
{
    public class Standard : ICourse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public int Duration { get; set; }
        public int MaxFunding { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public IEnumerable<FundingPeriod> FundingPeriods { get; set; }
    }
}