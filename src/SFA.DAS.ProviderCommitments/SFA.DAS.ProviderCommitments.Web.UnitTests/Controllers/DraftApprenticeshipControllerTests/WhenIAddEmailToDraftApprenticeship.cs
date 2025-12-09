namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests;

public class WhenIAddEmailToDraftApprenticeship
{
    private DraftApprenticeshipControllerTestFixture _fixture;

    [SetUp]
    public void Arrange()
    {
        _fixture = new DraftApprenticeshipControllerTestFixture();
    }

    [Test]
    public async Task Then_The_Email_Is_Saved_And_Redirected_To_ViewDraftApprenticeship()
    {
        _fixture.SetupAddEmailAddress();

        await _fixture.PostToAddEmail();

        _fixture
            .VerifyApiUpdateWithAddEmailSet()
            .VerifyRedirectedToEditDraftApprenticeship();
    }
}

