﻿using Microsoft.Extensions.Options;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class CommitmentsApiRegistry : Registry
    {
        public CommitmentsApiRegistry()
        {
            IncludeRegistry<CommitmentsApiClientRegistry>();
            For<ICommitmentsApiClientFactory>().Use("", x =>
            {
                var config = x.GetInstance<IOptions<AzureActiveDirectoryClientConfiguration>>().Value;
                return new CommitmentsApiClientFactory(config);
            });
        }
    }
}
