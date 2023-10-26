using Newtonsoft.Json;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class EncodingServiceRegistrations
{
    public static IServiceCollection AddEncodingServices(this IServiceCollection services, IConfiguration configuration) 
    {
        // To be confirmed if encodingServices are used at all or can be deleted?
        var encodingConfigJson = configuration.GetSection(ProviderCommitmentsConfigurationKeys.Encoding).Value;
        var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
        services.AddSingleton(encodingConfig);

        services.AddSingleton<IEncodingService, EncodingService>();

        return services;
    }
}