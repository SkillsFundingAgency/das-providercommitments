using MediatR;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.DependencyResolution
{
    public class MediatorRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.ProviderCommitments";

        public MediatorRegistry()
        {
            For<IMediator>().Use<Mediator>();
            For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceName));
                scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>));
                scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
            });
        }
    }
}
