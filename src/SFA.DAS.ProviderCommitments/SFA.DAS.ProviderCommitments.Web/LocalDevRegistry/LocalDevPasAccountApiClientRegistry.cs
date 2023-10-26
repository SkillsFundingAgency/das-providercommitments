using System;
using System.Linq.Expressions;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.LocalDevRegistry
{
    public class LocalDevPasAccountApiClientRegistry : Registry
    {
        public LocalDevPasAccountApiClientRegistry(Expression<Func<IContext, PasAccountApiConfiguration>> getApiConfig)
        {
            For<PasAccountApiConfiguration>().Use(getApiConfig);
            For<IPasAccountApiClient>().Use(ctx => CreateClient(ctx)).Singleton();
        }
        private IPasAccountApiClient CreateClient(IContext ctx)
        {
            var config = ctx.GetInstance<PasAccountApiConfiguration>();
            var loggerFactory = ctx.GetInstance<ILoggerFactory>();

            var factory = new LocalDevPasAccountApiClientFactory(config, loggerFactory);
            return factory.CreateClient();
        }
    }
}
