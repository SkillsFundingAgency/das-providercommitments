using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Controllers;

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
    
    [TestCase(typeof(IRequestHandler<DeleteCachedFileCommand>))]
    [TestCase(typeof(IRequestHandler<CreateCohortRequest, CreateCohortResponse>))]
    [TestCase(typeof(IRequestHandler<FileUploadValidateDataRequest>))]
    [TestCase(typeof(IRequestHandler<GetTrainingCoursesQueryRequest, GetTrainingCoursesQueryResponse>))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Mediator_Handlers(Type toResolve)
    {
        RunTestForType(toResolve);
    }

    private static void RunTestForType(Type toResolve)
    {
        var mockHostEnvironment = new Mock<IHostEnvironment>();
        mockHostEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);

        var startup = new Startup(GenerateStubConfiguration(), mockHostEnvironment.Object, false);
        var serviceCollection = new ServiceCollection();
        startup.ConfigureServices(serviceCollection);

        var mockHostingEnvironment = new Mock<IWebHostEnvironment>();
        mockHostEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);

        serviceCollection.AddSingleton(_ => mockHostingEnvironment.Object);
        
        serviceCollection.AddTransient<ApprenticeController>();
        serviceCollection.AddTransient<CohortController>();
        serviceCollection.AddTransient<DraftApprenticeshipController>();
        serviceCollection.AddTransient<OverlappingTrainingDateRequestController>();
        serviceCollection.AddTransient<ProviderAccountController>();
        
        var provider = serviceCollection.BuildServiceProvider();
        var type = provider.GetService(toResolve);
        
        Assert.IsNotNull(type);
    }

    private static IConfigurationRoot GenerateStubConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("SFA.DAS.Encoding", "{\"Encodings\": [{\"EncodingType\": \"AccountId\",\"Salt\": \"and vinegar\",\"MinHashLength\": 32,\"Alphabet\": \"46789BCDFGHJKLMNPRSTVWXY\"}]}"),

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
