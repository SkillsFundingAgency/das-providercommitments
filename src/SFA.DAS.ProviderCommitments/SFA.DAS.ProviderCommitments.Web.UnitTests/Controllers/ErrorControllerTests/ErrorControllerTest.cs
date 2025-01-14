using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Error;
using FluentAssertions.Execution;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ErrorControllerTests;

[TestFixture]
public class ErrorControllerTest
{
    private Mock<IConfiguration> _configuration;
    private ErrorController _sut;
    
    [TearDown]
    public void TearDown() => _sut.Dispose();

    [Test]
    [TestCase("test", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
    [TestCase("pp", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
    [TestCase("local", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
    [TestCase("prd", "https://services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
    public void Then_The_Page_Returns_HelpLink(string env, string helpLink, bool isActionRequest)
    {
        var fixture = new Fixture();

        _configuration = new Mock<IConfiguration>();
        _sut = new ErrorController(_configuration.Object);
            
        _configuration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
        
        var result = (ViewResult)_sut.Error(403, isActionRequest);
        result.ViewName.Should().Be("403");

        result.Should().NotBeNull();
        var actualModel = result?.Model as Error403ViewModel;
        
        using (new AssertionScope())
        {
            actualModel?.HelpPageLink.Should().Be(helpLink);
            actualModel?.IsActionRequest.Should().Be(isActionRequest);
        }
    }
}