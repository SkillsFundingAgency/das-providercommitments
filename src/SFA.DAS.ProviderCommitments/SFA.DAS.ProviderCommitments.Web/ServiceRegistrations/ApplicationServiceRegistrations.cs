using SFA.DAS.Authorization.Context;
using SFA.DAS.CommitmentsV2.Services.Shared;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Services;
using SFA.DAS.ProviderCommitments.Infrastructure;
using SFA.DAS.ProviderCommitments.Infrastructure.CacheStorageService;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Services;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IAcademicYearDateProvider, AcademicYearDateProvider>();
        services.AddTransient<IPolicyAuthorizationWrapper, PolicyAuthorizationWrapper>();
        services.AddTransient<IAuthorizationContextProvider, AuthorizationContextProvider>();
        services.AddTransient<IModelMapper, ModelMapper>();
        services.AddSingleton<ILinkGenerator, LinkGenerator>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<ICurrentDateTime, CurrentDateTime>();
        services.AddSingleton<ICreateCsvService, CreateCsvService>();
        
        services.AddSingleton<IBlobFileTransferClient, BlobFileTransferClient>();
        services.AddSingleton<ICacheService, CacheService>();
        services.AddTransient<ICacheStorageService, CacheStorageService>();
        services.AddTransient<ITempDataStorageService, TempDataStorageService>();
        services.AddTransient<IOuterApiClient, OuterApiClient>();
        services.AddTransient<IOuterApiService, OuterApiService>();
        
        return services;
    }
    
}