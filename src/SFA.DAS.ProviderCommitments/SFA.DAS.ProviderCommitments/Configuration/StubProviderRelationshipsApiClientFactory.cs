using System;
using SFA.DAS.CommitmentsV2.Shared.Services;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Api.Client.Http;

namespace SFA.DAS.ProviderCommitments.Configuration
{
    public class StubProviderRelationshipsApiClientFactory : IProviderRelationshipsApiClientFactory
    {
        public IProviderRelationshipsApiClient CreateApiClient()
        {
            return new StubProviderRelationshipsApiClient();
        }
    }
}