using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
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

            Assert.That(model, Is.EqualTo(fixture.CourseViewModel));
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
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly Mock<ITempDataDictionary> _tempDataMock;
        private readonly EditApprenticeshipRequestViewModel _apprenticeship;
        public readonly EditApprenticeshipCourseViewModel CourseViewModel;
        private readonly BaseApprenticeshipRequest _apprenticeshipRequestViewModel;

        public EditApprenticeshipRequest Request { get; }
        public ApprenticeController Sut { get; }

        public WhenSelectingCourseOnEditApprenticeshipFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Create<EditApprenticeshipRequest>();
            _apprenticeship = fixture.Build<EditApprenticeshipRequestViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x => x.StartDate)
                .Without(x => x.EndMonth).Without(x => x.EndYear).Without(x => x.EndDate)
                .Create();
            CourseViewModel = fixture.Create<EditApprenticeshipCourseViewModel>();
            _apprenticeshipRequestViewModel = fixture.Create<BaseApprenticeshipRequest>();

            _modelMapperMock = new Mock<IModelMapper>();
            _tempDataMock = new Mock<ITempDataDictionary>();

            var commitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            
            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), commitmentsApiClientMock.Object, Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>());
            Sut.TempData = _tempDataMock.Object;
            ;
        }

        public WhenSelectingCourseOnEditApprenticeshipFixture WithApprenticeship()
        {
            object asString = JsonConvert.SerializeObject(_apprenticeship);
            _tempDataMock.Setup(x => x.Peek(It.IsAny<string>())).Returns(asString);
            return this;
        }

        public WhenSelectingCourseOnEditApprenticeshipFixture SetApprenticeshipRequestViewModel()
        {
            _modelMapperMock.Setup(x => x.Map<BaseApprenticeshipRequest>(It.IsAny<EditApprenticeshipCourseViewModel>())).ReturnsAsync(_apprenticeshipRequestViewModel);
            return this;
        }

        public WhenSelectingCourseOnEditApprenticeshipFixture SetCourseViewModel()
        {
            _modelMapperMock.Setup(x => x.Map<EditApprenticeshipCourseViewModel>(It.IsAny<EditApprenticeshipRequest>())).ReturnsAsync(CourseViewModel);
            return this;
        }
    }
}
