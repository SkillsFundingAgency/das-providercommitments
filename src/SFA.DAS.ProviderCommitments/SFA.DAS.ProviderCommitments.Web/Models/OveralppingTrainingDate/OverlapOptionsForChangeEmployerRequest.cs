﻿using System;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

public class OverlapOptionsForChangeEmployerRequest : IAuthorizationContextModel
{
    public string ApprenticeshipHashedId { get; set; }
    public long? ApprenticeshipId { get; set; }
    public Guid CacheKey { get; set; }
    public long ProviderId { get; set; }
}