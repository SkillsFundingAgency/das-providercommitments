using StructureMap;

namespace SFA.DAS.Commitments.Shared.DependencyInjection
{
    public class ApprenticeshipInfoServiceRegistry : Registry
    {
        public ApprenticeshipInfoServiceRegistry()
        {
            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.Apprenticeships.Api.Client"));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });
        }
    }
}