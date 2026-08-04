using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests;

[TestFixture]
public class WhenGettingBeforeYouContinue
{
    [Test]
    public void ThenReturnsView()
    {
        var fixture = new WhenGettingBeforeYouContinueFixture();

        var result = fixture.Act();

        result.VerifyReturnsViewModel();
    }

    [Test]
    public void ThenProviderIdIsMapped()
    {
        var fixture = new WhenGettingBeforeYouContinueFixture();

        var result = fixture.Act();

        var model = result.VerifyReturnsViewModel().WithModel<BeforeYouContinueViewModel>();
        model.ProviderId.Should().Be(fixture.ProviderId);
    }

    [TestCase("true", true)]
    [TestCase("false", false)]
    public void ThenIlrSelectMultipleFeatureEnabledIsMappedFromConfiguration(string configValue, bool expected)
    {
        var fixture = new WhenGettingBeforeYouContinueFixture()
            .WithConfiguration("ILRSelectMultipleFeatureEnabled", configValue);

        var result = fixture.Act();

        var model = result.VerifyReturnsViewModel().WithModel<BeforeYouContinueViewModel>();
        model.IlrSelectMultipleFeatureEnabled.Should().Be(expected);
    }

    [Test]
    public void ThenIlrSelectMultipleFeatureEnabledDefaultsToFalseWhenConfigKeyIsAbsent()
    {
        var fixture = new WhenGettingBeforeYouContinueFixture()
            .WithEmptyConfiguration();

        var result = fixture.Act();

        var model = result.VerifyReturnsViewModel().WithModel<BeforeYouContinueViewModel>();
        model.IlrSelectMultipleFeatureEnabled.Should().BeFalse();
    }
}

public class WhenGettingBeforeYouContinueFixture
{
    public CohortController Sut { get; set; }

    private readonly BeforeYouContinueRequest _request;
    private IConfiguration _configuration = new ConfigurationBuilder().Build();
    public readonly long ProviderId = 123;

    public WhenGettingBeforeYouContinueFixture()
    {
        _request = new BeforeYouContinueRequest { ProviderId = ProviderId };
        Sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
            Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(), Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
    }

    public WhenGettingBeforeYouContinueFixture WithConfiguration(string key, string value)
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { [key] = value })
            .Build();
        return this;
    }

    public WhenGettingBeforeYouContinueFixture WithEmptyConfiguration()
    {
        _configuration = new ConfigurationBuilder().Build();
        return this;
    }

    public IActionResult Act() => Sut.BeforeYouContinue(_request, _configuration);
}
