﻿using SFA.DAS.Http.Configuration;

namespace SFA.DAS.ProviderCommitments.Configuration
{
    public class ApprovalsOuterApiConfiguration : IApimClientConfiguration
    {
        public string ApiBaseUrl { get; set; } = null!;
        public string SubscriptionKey { get; set; } = null!;
        public string ApiVersion { get; set; } = null!;
    }
}