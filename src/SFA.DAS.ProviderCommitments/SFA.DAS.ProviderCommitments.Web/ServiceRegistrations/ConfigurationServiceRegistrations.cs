﻿using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.AddConfigurationFor<CommitmentsClientApiConfiguration>(configuration, ProviderCommitmentsConfigurationKeys.CommitmentsClientApiConfiguration);
        services.AddConfigurationFor<ApprovalsOuterApiConfiguration>(configuration, ProviderCommitmentsConfigurationKeys.ApprovalsOuterApiConfiguration);
        services.AddConfigurationFor<CommitmentPermissionsApiClientConfiguration>(configuration, ProviderCommitmentsConfigurationKeys.CommitmentsClientApiConfiguration);
        services.AddConfigurationFor<ProviderFeaturesConfiguration>(configuration, ProviderCommitmentsConfigurationKeys.FeaturesConfiguration);
        services.AddConfigurationFor<ZenDeskConfiguration>(configuration, ProviderCommitmentsConfigurationKeys.ZenDeskConfiguration);
        services.AddConfigurationFor<DataProtectionConnectionStrings>(configuration, ProviderCommitmentsConfigurationKeys.DataProtectionConnectionStrings);
        services.AddConfigurationFor<BulkUploadFileValidationConfiguration>(configuration, ProviderCommitmentsConfigurationKeys.BulkUploadFileValidationConfiguration);
        services.AddConfigurationFor<ProviderSharedUIConfiguration>(configuration, ProviderCommitmentsConfigurationKeys.ProviderSharedUIConfiguration);
        services.AddConfigurationFor<BlobStorageSettings>(configuration, ProviderCommitmentsConfigurationKeys.BlobStorageSettings);
        services.AddConfigurationFor<PasAccountApiConfiguration>(configuration, ProviderCommitmentsConfigurationKeys.ProviderAccountApiConfiguration);

        return services;
    }

    private static void AddConfigurationFor<T>(this IServiceCollection services, IConfiguration configuration,
        string key) where T : class => services.AddSingleton(GetConfigurationFor<T>(configuration, key));

    private static T GetConfigurationFor<T>(IConfiguration configuration, string name)
    {
        var section = configuration.GetSection(name);
        return section.Get<T>();
    }
}