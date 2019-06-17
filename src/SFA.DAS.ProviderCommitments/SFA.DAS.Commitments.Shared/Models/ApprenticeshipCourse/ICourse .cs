using System;
using System.Collections.Generic;

namespace SFA.DAS.Commitments.Shared.Models.ApprenticeshipCourse
{
    public interface ICourse
    {
        string Id { get; set; }
        string Title { get; set; }
        int Level { get; set; }
        int MaxFunding { get; set; }
        DateTime? EffectiveFrom { get; set; }
        DateTime? EffectiveTo { get; set; }
        IEnumerable<FundingPeriod> FundingPeriods { get; set; }
    }
}