﻿using SFA.DAS.AutoConfiguration;
using SFA.DAS.CommitmentsV2.Services.Shared;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Services;
using SFA.DAS.ProviderCommitments.Infrastructure;
using SFA.DAS.ProviderCommitments.Infrastructure.CacheStorageService;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IModelMapper, ModelMapper>();
        
        services.AddSingleton<IAcademicYearDateProvider, AcademicYearDateProvider>();
        
        services.AddTransient<IAzureTableStorageConnectionAdapter, AzureTableStorageConnectionAdapter>();
        services.AddTransient<IEnvironmentService, EnvironmentService>();
        services.AddTransient<IAutoConfigurationService, TableStorageConfigurationService>();
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
        services.AddTransient<IOperationPermissionClaimsProvider, OperationPermissionClaimClaimsProvider>();
        
        services.AddTransient<IBulkUploadFileParser, BulkUploadFileParser>();
        
        services.AddTransient<ISelectDeliveryModelMapperHelper, SelectDeliveryModelMapperHelper>();
        services.AddTransient<ISelectCourseViewModelMapperHelper, SelectCourseViewModelMapperHelper>();
        
        services.AddSingleton(typeof(Interfaces.ICookieStorageService<>), typeof(Infrastructure.CookieService.CookieStorageService<>));

        return services;
    }
}