using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenSelectingCourseOnEditApprenticeship
    {
        [Test]
        public async Task Should_Return_View_With_Mapped_ViewModel()
        {
            var fixture = new WhenSelectingCourseOnEditApprenticeshipFixture()
                .WithDraftApprenticeship();

            var result = await fixture.Sut.EditDraftApprenticeshipCourse(fixture.Request);
            result.VerifyReturnsViewModel().ViewName.Should().Be(null);
            var model = result.VerifyReturnsViewModel().WithModel<EditDraftApprenticeshipCourseViewModel>();

            model.Should().Be(fixture.ViewModel);
        }

        [Test]
        public async Task Should_Redirect_To_Select_Delivery_Model_Page()
        {
            var fixture = new WhenSelectingCourseOnEditApprenticeshipFixture()
                .WithDraftApprenticeship();

            var result = await fixture.Sut.EditDraftApprenticeshipCourse(fixture.ViewModel);
            result.VerifyReturnsRedirectToActionResult().ActionName.Should().Be("SelectDeliveryModelForEdit");
        }
    }

    public class WhenSelectingCourseOnEditApprenticeshipFixture
    {
        private readonly Mock<ITempDataDictionary> _tempDataMock;
        private readonly EditDraftApprenticeshipViewModel _draftApprenticeship;

        public DraftApprenticeshipController Sut { get; }
        public DraftApprenticeshipRequest Request { get; }
        public EditDraftApprenticeshipCourseViewModel ViewModel { get; }

        public WhenSelectingCourseOnEditApprenticeshipFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Create<DraftApprenticeshipRequest>();
            _draftApprenticeship = fixture.Build<EditDraftApprenticeshipViewModel>().Without(x => x.BirthDay)
                .Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x => x.StartDate)
                .Without(x => x.EndMonth).Without(x => x.EndYear)
                .Create();
            var cohort = fixture.Create<GetCohortResponse>();
            ViewModel = fixture.Create<EditDraftApprenticeshipCourseViewModel>();

            var modelMapperMock = new Mock<IModelMapper>();
            _tempDataMock = new Mock<ITempDataDictionary>();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var mediatorMock = new Mock<IMediator>();

            modelMapperMock
                .Setup(x => x.Map<EditDraftApprenticeshipCourseViewModel>(It.IsAny<DraftApprenticeshipRequest>()))
                .ReturnsAsync(ViewModel);

            var commitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            commitmentsApiClientMock.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cohort);

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

        public WhenSelectingCourseOnEditApprenticeshipFixture WithDraftApprenticeship()
        {
            object asString = JsonConvert.SerializeObject(_draftApprenticeship);
            _tempDataMock.Setup(x => x.Peek(It.IsAny<string>())).Returns(asString);
            return this;
        }
    }
}