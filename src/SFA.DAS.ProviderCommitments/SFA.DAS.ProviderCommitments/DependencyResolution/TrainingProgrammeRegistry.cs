using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.ProviderCommitments.Configuration;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.DependencyResolution
{
    public class TrainingProgrammeRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.ProviderCommitments";

        public TrainingProgrammeRegistry()
        {
            // You'll also need to use the call AddMemoryCache in MVC startup to make IMemoryCache available
            For<IStandardApiClient>().Use<StandardApiClient>().Ctor<string>("baseUrl").Is(ctx => ctx.GetInstance<ApprenticeshipInfoServiceConfiguration>().BaseUrl);
            For<IFrameworkApiClient>().Use<FrameworkApiClient>().Ctor<string>("baseUrl").Is(ctx => ctx.GetInstance<ApprenticeshipInfoServiceConfiguration>().BaseUrl);

            For<ITrainingProgrammeApiClient>().Use<TrainingProgrammeApiClient>();
        }
    }
}