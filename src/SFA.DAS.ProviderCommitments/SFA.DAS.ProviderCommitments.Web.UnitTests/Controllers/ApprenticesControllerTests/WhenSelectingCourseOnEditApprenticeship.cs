using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenSelectingCourseOnEditApprenticeship
    {
        [Test]
        public async Task Should_Return_View_With_Mapped_ViewModel()
        {
            var fixture = new WhenSelectingCourseOnEditApprenticeshipFixture()
                .WithApprenticeship()
                .SetCourseViewModel();

            var result = await fixture.Sut.EditApprenticeshipCourse(fixture.Request);
            result.VerifyReturnsViewModel().ViewName.Should().Be(null);
            var model = result.VerifyReturnsViewModel().WithModel<EditApprenticeshipCourseViewModel>();

            Assert.AreEqual(fixture.CourseViewModel, model);
        }

        [Test]
        public async Task Should_Redirect_To_Select_Delivery_Model_Page()
        {
            var fixture = new WhenSelectingCourseOnEditApprenticeshipFixture()
                .WithApprenticeship()
                .SetApprenticeshipRequestViewModel();

            var result = await fixture.Sut.SetCourseForEdit(fixture.CourseViewModel);
            result.VerifyReturnsRedirectToActionResult().ActionName.Should().Be("SelectDeliveryModelForEdit");
        }
    }

    public class WhenSelectingCourseOnEditApprenticeshipFixture
    {
        public ApprenticeController Sut { get; set; }

        public string RedirectUrl;
        public Mock<IModelMapper> ModelMapperMock;
        public Mock<ITempDataDictionary> TempDataMock;
        public EditApprenticeshipRequest Request;
        public EditApprenticeshipRequestViewModel Apprenticeship;
        public GetApprenticeshipResponse ApprenticeshipResponse;
        public EditApprenticeshipCourseViewModel CourseViewModel;
        public BaseApprenticeshipRequest ApprenticeshipRequestViewModel;
        public Mock<ICommitmentsApiClient> CommitmentsApiClientMock;

        public WhenSelectingCourseOnEditApprenticeshipFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Create<EditApprenticeshipRequest>();
            Apprenticeship = fixture.Build<EditApprenticeshipRequestViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x => x.StartDate)
                .Without(x => x.EndMonth).Without(x => x.EndYear).Without(x => x.EndDate)
                .Create();
            ApprenticeshipResponse = fixture.Create<GetApprenticeshipResponse>();
            CourseViewModel = fixture.Create<EditApprenticeshipCourseViewModel>();
            ApprenticeshipRequestViewModel = fixture.Create<BaseApprenticeshipRequest>();

            ModelMapperMock = new Mock<IModelMapper>();
            TempDataMock = new Mock<ITempDataDictionary>();

            CommitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            
            Sut = new ApprenticeController(ModelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), CommitmentsApiClientMock.Object);
            Sut.TempData = TempDataMock.Object;
            ;
        }

        public WhenSelectingCourseOnEditApprenticeshipFixture WithApprenticeship()
        {
            object asString = JsonConvert.SerializeObject(Apprenticeship);
            TempDataMock.Setup(x => x.Peek(It.IsAny<string>())).Returns(asString);
            return this;
        }

        public WhenSelectingCourseOnEditApprenticeshipFixture SetApprenticeshipRequestViewModel()
        {
            ModelMapperMock.Setup(x => x.Map<BaseApprenticeshipRequest>(It.IsAny<EditApprenticeshipCourseViewModel>())).ReturnsAsync(ApprenticeshipRequestViewModel);
            return this;
        }

        public WhenSelectingCourseOnEditApprenticeshipFixture SetCourseViewModel()
        {
            ModelMapperMock.Setup(x => x.Map<EditApprenticeshipCourseViewModel>(It.IsAny<EditApprenticeshipRequest>())).ReturnsAsync(CourseViewModel);
            return this;
        }
    }
}
