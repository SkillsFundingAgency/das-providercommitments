using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Error;
using System;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ErrorControllerTests;

[TestFixture]
public class ErrorControllerTest
{
    private Mock<IConfiguration> _configuration;
    private bool _useDfESignIn;
    private ErrorController _sut;
    
    [TearDown]
    public void TearDown() => _sut.Dispose();

    [Test]
    [TestCase("test", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service")]
    [TestCase("pp", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service")]
    [TestCase("local", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service")]
    [TestCase("prd", "https://services.signin.education.gov.uk/approvals/select-organisation?action=request-service")]
    public void Then_The_Page_Returns_HelpLink(string env, string helpLink)
    {
        var fixture = new Fixture();

        _useDfESignIn = fixture.Create<bool>();

        _configuration = new Mock<IConfiguration>();
        _sut = new ErrorController(_configuration.Object);
            
        _configuration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
        _configuration.Setup(x => x["UseDfESignIn"]).Returns(Convert.ToString(_useDfESignIn));
        
        var result = (ViewResult)_sut.Error(403);
        result.ViewName.Should().Be("403");

        Assert.That(result, Is.Not.Null);
        var actualModel = result?.Model as Error403ViewModel;
        
        Assert.Multiple(() =>
        {
            Assert.That(actualModel?.HelpPageLink, Is.EqualTo(helpLink));
            Assert.That(_useDfESignIn, Is.EqualTo(actualModel?.UseDfESignIn));
        });
    }
}