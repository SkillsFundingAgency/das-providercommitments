using System;
using SFA.DAS.Authorization.CommitmentPermissions.Client;
using SFA.DAS.CommitmentsV2.Api.Client;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web
{
    public class LocalDevRegistry : Registry
    {
        public LocalDevRegistry()
        {
            var value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (value == "Development")
            {
                For<ICommitmentsApiClientFactory>().ClearAll().Use<LocalDevApiClientFactory>();
                For<ICommitmentPermissionsApiClientFactory>().ClearAll().Use<LocalDevApiClientFactory>();
            }
        }
    }
}