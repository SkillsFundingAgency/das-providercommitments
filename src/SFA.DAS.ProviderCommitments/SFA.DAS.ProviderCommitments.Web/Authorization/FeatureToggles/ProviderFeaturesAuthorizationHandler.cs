using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization.Errors;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Authorization.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles;

public class ProviderFeaturesAuthorizationHandler : IAuthorizationHandler
{
    public string Prefix => "ProviderFeature.";
        
    private readonly IFeatureTogglesService<ProviderFeatureToggle> _featureTogglesService;

    public ProviderFeaturesAuthorizationHandler(IFeatureTogglesService<ProviderFeatureToggle> featureTogglesService)
    {
        _featureTogglesService = featureTogglesService;
    }

    public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
    {
        var authorizationResult = new AuthorizationResult();

        if (options.Count <= 0)
        {
            return Task.FromResult(authorizationResult);
        }
        
        options.EnsureNoAndOptions();
        options.EnsureNoOrOptions();
                
        var feature = options.Single();
        var featureToggle = _featureTogglesService.GetFeatureToggle(feature);

        if (!featureToggle.IsEnabled)
        {
            authorizationResult.AddError(new ProviderFeatureNotEnabled());
        }
        else if (featureToggle.IsWhitelistEnabled)
        {
            var values = authorizationContext.GetProviderFeatureValues();

            if (!featureToggle.IsUserWhitelisted(values.Ukprn, values.UserEmail))
            {
                authorizationResult.AddError(new ProviderFeatureUserNotWhitelisted());
            }
        }

        return Task.FromResult(authorizationResult);
    }
}