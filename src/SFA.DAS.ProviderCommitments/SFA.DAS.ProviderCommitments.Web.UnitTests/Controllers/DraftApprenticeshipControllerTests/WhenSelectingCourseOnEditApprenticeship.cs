using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenSelectingCourseOnEditApprenticeship
    {
        [Test]
        public async Task GettingCourses_ShouldShowViewWithCourseSetAndCoursesListed()
        {
            var fixture = new WhenSelectingCourseOnEditApprenticeshipFixture()
                .WithDraftApprenticeship();

            var result = await fixture.Sut.SelectCourseForEdit(fixture.Request);
            result.VerifyReturnsViewModel().ViewName.Should().Be("SelectCourse");
            var model = result.VerifyReturnsViewModel().WithModel<SelectCourseViewModel>();
            model.CourseCode.Should().Be(fixture.DraftApprenticeship.CourseCode);
            model.Courses.Should().BeEquivalentTo(fixture.TrainingCourseResponse.TrainingCourses);
        }

        [Test]
        public async Task WhenSettingCourse_AndNoCourseSelected_ShouldThrowException()
        {
            var fixture = new WhenSelectingCourseOnEditApprenticeshipFixture();
            fixture.ViewModel.CourseCode = null;

            try
            {
                var result = await fixture.Sut.SetCourseForEdit(fixture.ViewModel);
                Assert.Fail("Should have had exception thrown");
            }
            catch (CommitmentsApiModelException e)
            {
                e.Errors[0].Field.Should().Be("CourseCode");
                e.Errors[0].Message.Should().Be("Please select a course");
            }
        }

        [Test]
        public async Task WhenSettingCourse_AndCourseSelected_ShouldRedirectToEditDraftApprenticeship()
        {
            var fixture = new WhenSelectingCourseOnEditApprenticeshipFixture()
                .WithDraftApprenticeship();

            fixture.ViewModel.CourseCode = "123";

            var result = await fixture.Sut.SetCourseForEdit(fixture.ViewModel);
            result.VerifyReturnsRedirectToActionResult().ActionName.Should().Be("SelectDeliveryModelForEdit");
        }
    }

    public class WhenSelectingCourseOnEditApprenticeshipFixture
    {
        public DraftApprenticeshipController Sut { get; set; }

        public string RedirectUrl;
        public Mock<IModelMapper> ModelMapperMock;
        public Mock<IAuthorizationService> AuthorizationServiceMock;
        public Mock<ITempDataDictionary> TempDataMock;
        public Mock<IMediator> MediatorMock;
        public SelectCourseViewModel ViewModel;
        public DraftApprenticeshipRequest Request;
        public EditDraftApprenticeshipViewModel DraftApprenticeship;
        public GetCohortResponse Cohort;
        public GetTrainingCoursesQueryResponse TrainingCourseResponse;
        public Mock<ICommitmentsApiClient> CommitmentsApiClientMock;

        public WhenSelectingCourseOnEditApprenticeshipFixture()
        {
            var fixture = new Fixture();
            ViewModel = fixture.Create<SelectCourseViewModel>();
            Request = fixture.Create<DraftApprenticeshipRequest>();
            DraftApprenticeship = fixture.Build<EditDraftApprenticeshipViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x => x.StartDate)
                .Without(x => x.EndMonth).Without(x => x.EndYear)
                .Create();
            Cohort = fixture.Create<GetCohortResponse>();
            TrainingCourseResponse = fixture.Create<GetTrainingCoursesQueryResponse>();

            ModelMapperMock = new Mock<IModelMapper>();
            TempDataMock = new Mock<ITempDataDictionary>();
            AuthorizationServiceMock = new Mock<IAuthorizationService>();
            MediatorMock = new Mock<IMediator>();
            MediatorMock.Setup(x => x.Send(It.IsAny<GetTrainingCoursesQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TrainingCourseResponse);

            CommitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            CommitmentsApiClientMock.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(Cohort);

            Sut = new DraftApprenticeshipController(MediatorMock.Object, CommitmentsApiClientMock.Object, ModelMapperMock.Object, Mock.Of<IEncodingService>(), AuthorizationServiceMock.Object);
            Sut.TempData = TempDataMock.Object;
        }

        public WhenSelectingCourseOnEditApprenticeshipFixture WithDraftApprenticeship()
        {
            object asString = JsonConvert.SerializeObject(DraftApprenticeship);
            TempDataMock.Setup(x => x.Peek(It.IsAny<string>())).Returns(asString);
            return this;
        }

        public void VerifyReturnsRedirect(IActionResult redirectResult)
        {
            redirectResult.VerifyReturnsRedirect().Url.Equals(RedirectUrl);
        }
    }
}
