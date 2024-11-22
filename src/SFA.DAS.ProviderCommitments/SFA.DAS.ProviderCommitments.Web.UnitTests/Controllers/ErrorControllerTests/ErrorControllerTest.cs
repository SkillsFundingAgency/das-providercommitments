using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Error;
using System;
using FluentAssertions.Execution;

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
    [TestCase("test", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
    [TestCase("pp", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
    [TestCase("local", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
    [TestCase("prd", "https://services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
    public void Then_The_Page_Returns_HelpLink(string env, string helpLink, bool isPostRequest)
    {
        var fixture = new Fixture();

        _useDfESignIn = fixture.Create<bool>();

        _configuration = new Mock<IConfiguration>();
        _sut = new ErrorController(_configuration.Object);
            
        _configuration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
        _configuration.Setup(x => x["UseDfESignIn"]).Returns(Convert.ToString(_useDfESignIn));
        
        var result = (ViewResult)_sut.Error(403, isPostRequest);
        result.ViewName.Should().Be("403");

        result.Should().NotBeNull();
        var actualModel = result?.Model as Error403ViewModel;
        
        using (new AssertionScope())
        {
            actualModel?.HelpPageLink.Should().Be(helpLink);
            _useDfESignIn.Should().Be((bool)actualModel?.UseDfESignIn);
            actualModel?.IsPostRequest.Should().Be(isPostRequest);
        }
    }
}