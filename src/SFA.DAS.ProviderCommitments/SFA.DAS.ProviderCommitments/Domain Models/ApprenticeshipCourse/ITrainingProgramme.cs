using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse
{
    public interface ITrainingProgramme
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