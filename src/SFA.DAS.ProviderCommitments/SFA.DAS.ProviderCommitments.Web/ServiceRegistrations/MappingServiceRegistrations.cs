using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class MappingServiceRegistrations
{
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        var mappingAssembly = typeof(ChangeVersionViewModelMapper).Assembly;
        
        var mappingTypes = mappingAssembly
            .GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<,>)));
        
        foreach (var mapperType in mappingTypes)
        {
            var mapperInterface = mapperType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<,>));
           
            services.AddTransient(mapperInterface, mapperType);
            services.Decorate(mapperType, _ => typeof(AttachUserInfoToSaveRequests<,>));
            services.Decorate(mapperType, _ => typeof(AttachApimUserInfoToSaveRequests<,>));
        }

        return services;
    }
}