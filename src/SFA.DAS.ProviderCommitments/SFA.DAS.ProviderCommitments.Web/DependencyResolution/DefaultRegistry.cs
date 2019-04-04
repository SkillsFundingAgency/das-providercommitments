using FluentValidation;
using MediatR;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Caching;
using SFA.DAS.ProviderCommitments.Infrastructure;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.ModelBinding.Interfaces;
using SFA.DAS.ProviderCommitments.ModelBinding.ModelBinderValues;
using SFA.DAS.ProviderUrlHelper;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.ProviderCommitments";

        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceName));
                    scan.ConnectImplementationsToTypesClosing(typeof(IValidator<>));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
            For<IMediator>().Use<Mediator>();
            For<ICache>().Use<InMemoryCache>().Singleton();
            For<ICurrentDateTime>().Use<CurrentDateTime>().Singleton();
            For<IHashingContextProvider>().Use<ModelBindingHashValuesProvider>();
            For<ILinkGenerator>().Use<LinkGenerator>();
        }
    }
}
