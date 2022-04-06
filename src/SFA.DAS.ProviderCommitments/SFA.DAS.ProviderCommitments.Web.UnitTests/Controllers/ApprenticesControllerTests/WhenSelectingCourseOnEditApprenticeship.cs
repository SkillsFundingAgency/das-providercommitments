﻿using System.Threading;
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
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenSelectingCourseOnEditApprenticeship
    {
        [Test]
        public async Task GettingCourses_ShouldShowViewWithCourseSetAndCoursesListed()
        {
            var fixture = new WhenSelectingCourseOnEditApprenticeshipFixture()
                .WithTempViewModel();

            var result = await fixture.Sut.SelectCourseForEdit(fixture.Request);
            result.VerifyReturnsViewModel().ViewName.Should().Be("SelectCourse");
            var model = result.VerifyReturnsViewModel().WithModel<SelectCourseViewModel>();
            model.CourseCode.Should().Be(fixture.ViewModel.CourseCode);
            model.Courses.Should().BeEquivalentTo(fixture.ViewModel.Courses);
        }

        [Test]
        public void WhenSettingCourse_AndNoCourseSelected_ShouldThrowException()
        {
            var fixture = new WhenSelectingCourseOnEditApprenticeshipFixture();
            fixture.ViewModel.CourseCode = null;

            try
            {
                var result = fixture.Sut.SetCourseForEdit(fixture.ViewModel);
                Assert.Fail("Should have had exception thrown");
            }
            catch (CommitmentsApiModelException e)
            {
                e.Errors[0].Field.Should().Be("CourseCode");
                e.Errors[0].Message.Should().Be("You must select a training course");
            }
        }

        [Test]
        public void WhenSettingCourse_AndCourseSelected_ShouldRedirectToEditApprenticeship()
        {
            var fixture = new WhenSelectingCourseOnEditApprenticeshipFixture().WithTempViewModel();

            fixture.ViewModel.CourseCode = "123";

            var result = fixture.Sut.SetCourseForEdit(fixture.ViewModel);
            result.VerifyReturnsRedirectToActionResult().ActionName.Should().Be("SelectDeliveryModelForEdit");
        }
    }

    public class WhenSelectingCourseOnEditApprenticeshipFixture
    {
        public ApprenticeController Sut { get; set; }

        public string RedirectUrl;
        public Mock<IModelMapper> ModelMapperMock;
        public Mock<ITempDataDictionary> TempDataMock;
        public SelectCourseViewModel ViewModel;
        public EditApprenticeshipRequest Request;
        public EditApprenticeshipRequestViewModel Apprenticeship;
        public GetCohortResponse Cohort;
        public GetTrainingCoursesQueryResponse TrainingCourseResponse;
        public Mock<ICommitmentsApiClient> CommitmentsApiClientMock;

        public WhenSelectingCourseOnEditApprenticeshipFixture()
        {
            var fixture = new Fixture();
            ViewModel = fixture.Create<SelectCourseViewModel>();
            Request = fixture.Create<EditApprenticeshipRequest>();
            Apprenticeship = fixture.Build<EditApprenticeshipRequestViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x => x.StartDate)
                .Without(x=>x.EndDate).Without(x => x.EndMonth).Without(x => x.EndMonth).Without(x => x.EndYear)
                .Create();

            Cohort = fixture.Create<GetCohortResponse>();
            TrainingCourseResponse = fixture.Create<GetTrainingCoursesQueryResponse>();

            ModelMapperMock = new Mock<IModelMapper>();
            ModelMapperMock.Setup(x => x.Map<SelectCourseViewModel>(It.IsAny<EditApprenticeshipRequestViewModel>())).ReturnsAsync(ViewModel);

            TempDataMock = new Mock<ITempDataDictionary>();

            CommitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            CommitmentsApiClientMock.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(Cohort);

            Sut = new ApprenticeController(ModelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), CommitmentsApiClientMock.Object);
            Sut.TempData = TempDataMock.Object;
        }

        public WhenSelectingCourseOnEditApprenticeshipFixture WithTempViewModel()
        {
            object asString = JsonConvert.SerializeObject(Apprenticeship);
            TempDataMock.Setup(x => x.Peek(It.IsAny<string>())).Returns(asString);
            return this;
        }

        public void VerifyReturnsRedirect(IActionResult redirectResult)
        {
            redirectResult.VerifyReturnsRedirect().Url.Equals(RedirectUrl);
        }
    }
}