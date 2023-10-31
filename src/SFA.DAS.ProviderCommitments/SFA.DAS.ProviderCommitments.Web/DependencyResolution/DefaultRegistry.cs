using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution;

public class DefaultRegistry : Registry
{
    private const string ServiceName = "SFA.DAS.ProviderCommitments";

    public DefaultRegistry()
    {
        Scan(
            scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceName));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });

        For(typeof(IMapper<,>)).DecorateAllWith(typeof(AttachUserInfoToSaveRequests<,>));
        For(typeof(IMapper<,>)).DecorateAllWith(typeof(AttachApimUserInfoToSaveRequests<,>));
    }
}