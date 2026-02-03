using System;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

public class GetCoursesResponse
{
    public IEnumerable<TrainingCourse> TrainingProgrammes { get; set; }
}

public class TrainingCourse
{
    public string CourseCode { get; set; }
    public string Name { get; set; }
    public string StandardUId { get; set; }
    public string Version { get; set; }
    public ProgrammeType ProgrammeType { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string StandardPageUrl { get; set; }
    public List<string> Options { get; set; }
    public List<TrainingProgrammeFundingPeriod> FundingPeriods { get; set; }
}

public class TrainingProgrammeFundingPeriod
{
    public int FundingCap { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public DateTime? EffectiveFrom { get; set; }
}