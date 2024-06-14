using SFA.DAS.Provider.Shared.UI.Models.Flags;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

[TestFixture]
public class WhenGettingApprenticeDetails
{
    private ApprenticeControllerTestFixtureBase _fixture;

    [SetUp]
    public void Arrange()
    {
        _fixture = new ApprenticeControllerTestFixtureBase();
    }

    [Test]
    public async Task AndWhenIGetDetails_Request_ToViewModelsMapped()
    {
        await _fixture.GetDetails();

        _fixture.VerifyModelMapperDetailsRequest_ToViewModelIsCalled();
    }

    [Test]
    public async Task ThenReturnsView()
    {
        await _fixture.GetDetails();

        _fixture.VerifyDetailViewReturned();
    }

    [TestCase(ApprenticeDetailsBanners.ChangeOfPriceApproved)]
    [TestCase(ApprenticeDetailsBanners.ChangeOfPriceApproved | ApprenticeDetailsBanners.ChangeOfStartDateSent)]
    public async Task ThenBannerFlagsAreMapped(ApprenticeDetailsBanners banners)
    {
        await _fixture.GetDetails(banners);

        _fixture.VerifyBannerFlagsAreMapped(banners);
    }
}