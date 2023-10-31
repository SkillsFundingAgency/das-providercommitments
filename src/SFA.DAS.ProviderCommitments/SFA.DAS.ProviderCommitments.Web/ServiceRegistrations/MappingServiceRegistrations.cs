using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class MappingServiceRegistrations
{
    // Getting this working should allow for the complete removal of StructureMap from the solution.
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
            
            // This one is not like the others ....
            if (mapperInterface != typeof(IMapper<CreateCohortRequest, CreateCohortApimRequest>))
            {
                DecorateImplementation(services, mapperInterface);;
            }
        }

        return services;
    }

    private static void DecorateImplementation(IServiceCollection services, Type mapperInterface)
    {
        services.Decorate(mapperInterface, _ => typeof(AttachUserInfoToSaveRequests<,>));
        services.Decorate(mapperInterface, _ => typeof(AttachApimUserInfoToSaveRequests<,>));
    }
}