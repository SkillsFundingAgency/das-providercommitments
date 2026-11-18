using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectMultipleFundingChoiceViewModel
{
    public long ProviderId { get; set; }
    public Guid CacheKey { get; set; }
    public SelectMultipleFundingChoice Selection { get; set; }
}

public enum SelectMultipleFundingChoice
{
    AllNew = 1,
    Existing = 2
}
