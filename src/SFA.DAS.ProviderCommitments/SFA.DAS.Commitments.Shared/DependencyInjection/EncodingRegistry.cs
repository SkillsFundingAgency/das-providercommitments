using SFA.DAS.Encoding;
using StructureMap;

namespace SFA.DAS.Commitments.Shared.DependencyInjection
{
    public class EncodingRegistry : Registry
    {
        public EncodingRegistry()
        {
            For<IEncodingService>().Use<EncodingService>().Singleton();
        }
    }
}