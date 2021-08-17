using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Configuration;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Services.Shared;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Services;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderUrlHelper;
using StructureMap;
using StructureMap.Building.Interception;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.ProviderCommitments";

        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceName));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For(typeof(IMapper<,>)).DecorateAllWith(typeof(AttachUserInfoToSaveRequests<,>));
            For<IModelMapper>().Use<ModelMapper>();
            For<IFeatureTogglesService<ProviderFeatureToggle>>().Use<FeatureTogglesService<ProviderFeaturesConfiguration, ProviderFeatureToggle>>();
            For<IAuthenticationService>().Use<AuthenticationService>().Singleton();
            For<IAuthorizationContextProvider>().Use<AuthorizationContextProvider>();
            For<ILinkGenerator>().Use<LinkGenerator>().Singleton();
            For<IPolicyAuthorizationWrapper>().Use<PolicyAuthorizationWrapper>();
            For<IAcademicYearDateProvider>().Use<AcademicYearDateProvider>().Singleton();
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>)).Singleton();
            For(typeof(HttpContext)).Use(c => c.GetInstance<IHttpContextAccessor>().HttpContext);

            Toggle<IProviderRelationshipsApiClient, StubProviderRelationshipsApiClient>("UseStubProviderRelationships");
        }
        
        private void Toggle<TPluginType, TConcreteType>(string configurationKey) where TConcreteType : TPluginType
        {
            For<TPluginType>().InterceptWith(new FuncInterceptor<TPluginType>((c, o) => c.GetInstance<IConfiguration>().GetValue<bool>(configurationKey) ? c.GetInstance<TConcreteType>() : o));
        }
    }
}
