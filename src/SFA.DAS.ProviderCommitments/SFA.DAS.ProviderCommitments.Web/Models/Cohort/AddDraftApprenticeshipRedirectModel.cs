using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class AddDraftApprenticeshipRedirectModel
{
    public long ProviderId { get; set; }
    public Guid CacheKey { get; set; }
    public bool IsEdit { get; set; }
}