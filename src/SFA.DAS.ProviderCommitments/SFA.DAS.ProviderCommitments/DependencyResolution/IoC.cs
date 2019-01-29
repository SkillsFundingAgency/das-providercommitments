using StructureMap;

namespace SFA.DAS.ProviderCommitments.DependencyResolution
{
    public static class IoC
    {
        public static void Initialize(Registry registry)
        {
            registry.IncludeRegistry<DefaultRegistry>();
        }
    }
}
