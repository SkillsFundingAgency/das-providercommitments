using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenAddOrUpdateEmailOnEditApprenticeship
    {
        [Test]
        public async Task Should_Return_View_With_Mapped_ViewModel()
        {
            var fixture = new WhenAddOrUpdateEmailOnEditApprenticeshipFixture();
            var result = await fixture.Sut.AddEmail(fixture.Request);
            result.VerifyReturnsViewModel().ViewName.Should().Be(null);
            var model = result.VerifyReturnsViewModel().WithModel<DraftApprenticeshipAddEmailViewModel>();

            model.Should().Be(fixture.ViewModel);
        }

        [Test]
        public async Task Should_Redirect_To_Edit_Draft_Apprenticeship_Page()
        {
            var fixture = new WhenAddOrUpdateEmailOnEditApprenticeshipFixture();

            var result = await fixture.Sut.AddEmail(fixture.ViewModel);
            result.VerifyReturnsRedirectToActionResult().ActionName.Should().Be("EditDraftApprenticeship");
        }
    }

    public class WhenAddOrUpdateEmailOnEditApprenticeshipFixture
    {
        private readonly Mock<ITempDataDictionary> _tempDataMock;
        public DraftApprenticeshipController Sut { get; }
        public DraftApprenticeshipAddEmailRequest Request { get; }
        public DraftApprenticeshipAddEmailViewModel ViewModel { get; }

        public WhenAddOrUpdateEmailOnEditApprenticeshipFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Create<DraftApprenticeshipAddEmailRequest>();
            ViewModel = fixture.Create<DraftApprenticeshipAddEmailViewModel>();

            var modelMapperMock = new Mock<IModelMapper>();
            _tempDataMock = new Mock<ITempDataDictionary>();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var mediatorMock = new Mock<IMediator>();

            modelMapperMock
                .Setup(x => x.Map<DraftApprenticeshipAddEmailViewModel>(It.IsAny<DraftApprenticeshipAddEmailRequest>()))
                .ReturnsAsync(ViewModel);

            var commitmentsApiClientMock = new Mock<ICommitmentsApiClient>();           

            Sut = new DraftApprenticeshipController(
                mediatorMock.Object,
                commitmentsApiClientMock.Object,
                modelMapperMock.Object,
                Mock.Of<IEncodingService>(),
                authorizationServiceMock.Object,
                Mock.Of<IOuterApiService>(),
                Mock.Of<IAuthenticationService>(),
                Mock.Of<ICacheStorageService>()
                );
            
            Sut.TempData = _tempDataMock.Object;
        }       
    }
}