﻿using AutoFixture;
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
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderCommitments.Web.Authentication;

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

            Assert.AreEqual(fixture.ViewModel, model);
        }

        [Test]
        public async Task Should_Redirect_To_Select_Delivery_Model_Page()
        {
            var fixture = new WhenSelectingCourseOnEditApprenticeshipFixture()
                .WithDraftApprenticeship();

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
        public DraftApprenticeshipRequest Request;
        public EditDraftApprenticeshipViewModel DraftApprenticeship;
        public GetCohortResponse Cohort;
        public EditDraftApprenticeshipCourseViewModel ViewModel;
        public Mock<ICommitmentsApiClient> CommitmentsApiClientMock;

        public WhenSelectingCourseOnEditApprenticeshipFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Create<DraftApprenticeshipRequest>();
            DraftApprenticeship = fixture.Build<EditDraftApprenticeshipViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x => x.StartDate)
                .Without(x => x.EndMonth).Without(x => x.EndYear)
                .Create();
            Cohort = fixture.Create<GetCohortResponse>();
            ViewModel = fixture.Create<EditDraftApprenticeshipCourseViewModel>();

            ModelMapperMock = new Mock<IModelMapper>();
            TempDataMock = new Mock<ITempDataDictionary>();
            AuthorizationServiceMock = new Mock<IAuthorizationService>();
            MediatorMock = new Mock<IMediator>();

            ModelMapperMock.Setup(x => x.Map<EditDraftApprenticeshipCourseViewModel>(It.IsAny<DraftApprenticeshipRequest>())).ReturnsAsync(ViewModel);
                
            CommitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            CommitmentsApiClientMock.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(Cohort);

            Sut = new DraftApprenticeshipController(
                MediatorMock.Object,
                CommitmentsApiClientMock.Object,
                ModelMapperMock.Object,
                Mock.Of<IEncodingService>(),
                AuthorizationServiceMock.Object,
                Mock.Of<IOuterApiService>(),
                Mock.Of<IAuthenticationService>(),
                Mock.Of<ILogger<DraftApprenticeshipController>>());
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
