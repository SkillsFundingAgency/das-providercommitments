using System;
using System.Collections.Generic;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Authorization.CommitmentPermissions.Configuration;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.Authorization.ProviderFeatures.Configuration;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;
using SFA.DAS.ProviderRelationships.Api.Client.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(ApprenticeController))]
    [TestCase(typeof(CohortController))]
    [TestCase(typeof(DraftApprenticeshipController))]
    [TestCase(typeof(OverlappingTrainingDateRequestController))]
    [TestCase(typeof(ProviderAccountController))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Controllers(Type toResolve)
    {
        RunTestForType(toResolve);
    }

    [TestCase(typeof(AuthenticationSettings))]
    [TestCase(typeof(CommitmentsClientApiConfiguration))]
    [TestCase(typeof(ApprovalsOuterApiConfiguration))]
    [TestCase(typeof(CommitmentPermissionsApiClientConfiguration))]
    [TestCase(typeof(ProviderRelationshipsApiConfiguration))]
    [TestCase(typeof(ProviderFeaturesConfiguration))]
    [TestCase(typeof(ZenDeskConfiguration))]
    [TestCase(typeof(DataProtectionConnectionStrings))]
    [TestCase(typeof(BulkUploadFileValidationConfiguration))]
    [TestCase(typeof(ProviderSharedUIConfiguration))]
    [TestCase(typeof(BlobStorageSettings))]
    [TestCase(typeof(PasAccountApiConfiguration))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Configuration(Type toResolve)
    {
        RunTestForType(toResolve);
    }

    [TestCase(typeof(IRequestHandler<DeleteCachedFileCommand>))]
    [TestCase(typeof(IRequestHandler<CreateCohortRequest, CreateCohortResponse>))]
    //[TestCase(typeof(IRequestHandler<FileUploadValidateDataRequest>))]
    [TestCase(typeof(IRequestHandler<GetTrainingCoursesQueryRequest, GetTrainingCoursesQueryResponse>))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Mediator_Handlers(Type toResolve)
    {
        RunTestForType(toResolve);
    }

    private static void RunTestForType(Type toResolve)
    {
        var mockHostEnvironment = new Mock<IHostEnvironment>();
        mockHostEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);

        var stubConfiguration = GenerateStubConfiguration();
        var services = new ServiceCollection();

        services.AddHttpClient();
        services.AddSingleton<IConfiguration>(stubConfiguration);
        services.AddHttpContextAccessor();
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<CreateCohortHandler>());
        
        services.AddConfigurationOptions(stubConfiguration);
        services.AddProviderAuthentication(stubConfiguration);
        services.AddMemoryCache();
        services.AddCache(mockHostEnvironment.Object, stubConfiguration);
        services.AddModelMappings();
        services.AddApprovalsOuterApiClient();
        services.AddAuthorization<AuthorizationContextProvider>();
        services.AddTransient<IValidator<CreateCohortRequest>, CreateCohortValidator>();
        services.AddTransient<IAuthenticationServiceForApim, AuthenticationService>();
        
        services
            .AddCommitmentsApiClient(stubConfiguration)
            .AddProviderApprenticeshipsApiClient(stubConfiguration);

        services.AddEncodingServices(stubConfiguration);
        services.AddApplicationServices();

        services.AddTransient<ApprenticeController>();
        services.AddTransient<CohortController>();
        services.AddTransient<DraftApprenticeshipController>();
        services.AddTransient<OverlappingTrainingDateRequestController>();
        services.AddTransient<ProviderAccountController>();
        
        var provider = services.BuildServiceProvider();
        var type = provider.GetService(toResolve);

        Assert.IsNotNull(type);
    }

    private static IConfigurationRoot GenerateStubConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("SFA.DAS.Encoding",
                    "{\"Encodings\": [{\"EncodingType\": \"AccountId\",\"Salt\": \"and vinegar\",\"MinHashLength\": 32,\"Alphabet\": \"46789BCDFGHJKLMNPRSTVWXY\"}]}"),

                new("APPINSIGHTS_INSTRUMENTATIONKEY", "test"),

                new("AuthenticationSettings:MetadataAddress", "https://test.com/"),
                new("AuthenticationSettings:Wtrealm", "https://test.com/"),
                new("AuthenticationSettings:UseStub", "true/"),

                new("CommitmentsClientApi:IdentifierUri", "https://test.com/"),
                new("CommitmentsClientApi:ApiBaseUrl", "https://test.com/"),

                new("ApprovalsOuterApi:ApiBaseUrl", "https://test.com/"),
                new("ApprovalsOuterApi:SubscriptionKey", "keyValue"),
                new("ApprovalsOuterApi:ApiVersion", "1"),

                new("ProviderRelationshipsApi:IdentifierUri", "https://test.com/"),
                new("ProviderRelationshipsApi:ApiBaseUrl", "https://test.com/"),

                new("ProviderAccountApiConfiguration:IdentifierUri", "https://test.com/"),
                new("ProviderAccountApiConfiguration:ApiBaseUrl", "https://test.com/"),

                new("Features:FeatureToggles", "test"),

                new("ZenDesk:SectionId", "ABC123"),
                new("ZenDesk:SnippetKey", "ABC123"),
                new("ZenDesk:CobrowsingSnippetKey", "ABC123"),

                new("DataProtection:RedisConnectionString", "test"),
                new("DataProtection:DataProtectionKeysDatabase", "test"),

                new("BulkUploadFileValidationConfiguration:MaxBulkUploadFileSize", "1"),
                new("BulkUploadFileValidationConfiguration:AllowedFileColumnCount", "1"),
                new("BulkUploadFileValidationConfiguration:MaxAllowedFileRowCount", "1"),

                new("ProviderSharedUIConfiguration:DashboardUrl", "https://test.com/"),

                new("BlobStorage:ConnectionString", "test"),
                new("BlobStorage:BulkUploadContainer", "test"),
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}