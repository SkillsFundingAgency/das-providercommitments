using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Infrastructure.FeatureDefinition;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web;

namespace SFA.DAS.ProviderCommitments.IntegrationTests
{
    [TestFixture]
    [Ignore("Integration tests are failing because IActionContextAccessor is not available, possibly because attempting to bypass auth")]
    public class FeatureRoutingTests
    {
        [TestCase("/10005077/unapproved/add-apprentice", "{providerId}/unapproved/add-apprentice", false)]
        [TestCase("/10005077/unapproved/add-apprentice", "{providerId}/unapproved/add-apprentice", false)]
        [TestCase("/10005077/unapproved/add-apprentice?reservationId=54801bfa-b9e7-4426-a649-6ef718bdce46&StartMonthYear=122019&EmployerAccountLegalEntityPublicHashedId=DJWWDJ&EmployerAccountPublicHashedId=WYLVND", "{providerId}/unapproved/add-apprentice", false)]
        public async Task Get_SpecifiedEndpoint_ShouldRespectFeatureToggle(string url, string urlToDisabledFeature, bool expectedFound)
        {
            // Arrange
            var fixtures = new FeatureRoutingTestFixtures()
                .AddDisabledFeature("TestFeature", urlToDisabledFeature);
                            
            var client  = fixtures.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Act
            var response = await client.GetAsync(url);

            // Assert
            if (expectedFound)
            {
                Assert.AreNotEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }

    public class FeatureRoutingTestFixtures : WebApplicationFactory<Startup>
    {

        public FeatureRoutingTestFixtures()
        {
            FeatureConfiguration = new FeatureConfiguration();
        }

        public FeatureConfiguration FeatureConfiguration { get; }

        public FeatureRoutingTestFixtures AddDisabledFeature(string name, params string[] endpoints)
        {
            return AddFeature(name, false, endpoints);
        }

        public FeatureRoutingTestFixtures AddEnabledFeature(string name, params string[] endpoints)
        {
            return AddFeature(name, true, endpoints);
        }

        public FeatureRoutingTestFixtures AddFeature(string name, bool enable, params string[] endpoints)
        {
            FeatureConfiguration.FeatureDefinitions = CopyAndAppend(FeatureConfiguration.FeatureDefinitions,
                new FeatureDefinition
                {
                    Endpoints = endpoints,
                    Name = name
                });

            if (enable)
            {
                FeatureConfiguration.EnabledFeatures = CopyAndAppend(FeatureConfiguration.EnabledFeatures, name);
            }

            return this;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.Add(new ServiceDescriptor(typeof(FeatureConfiguration), FeatureConfiguration));
                services.Add(new ServiceDescriptor(typeof(IFeatures), typeof(Features), ServiceLifetime.Singleton));

                services.BuildServiceProvider();
            });
        }

        private T[] CopyAndAppend<T>(T[] existingArray, T newInstance) 
        {
            var requiredSize = existingArray.Length + 1;

            var newArray = new T[requiredSize];

            existingArray.CopyTo(newArray, 0);

            newArray[requiredSize - 1] = newInstance;

            return newArray;
        }
    }
}