using System.Collections.Generic;
using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Queries.GetProviderCourseDeliveryModels;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenSelectingDeliveryModel
    {
        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithOnlyOneOption_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelFixture()
                .WithDeliveryModels(new List<DeliveryModel> {DeliveryModel.Normal});

            var result = await fixture.Sut.SelectDeliveryModel(fixture.Request) as RedirectToActionResult;
            result.ActionName.Should().Be("AddDraftApprenticeship");
        }

        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithMultipleOptions_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Normal, DeliveryModel.Flexible });

            var result = await fixture.Sut.SelectDeliveryModel(fixture.Request) as ViewResult;
            result.ViewName.Should().Be("SelectDeliveryModel");
        }

        [Test]
        public async Task WhenSettingDeliveryModel_AndNoOptionSet_ShouldThrowException()
        {
            var fixture = new WhenSelectingDeliveryModelFixture();
            fixture.ViewModel.DeliveryModel = null;

            try
            {
                var result = await fixture.Sut.SetDeliveryModel(fixture.ViewModel);
                Assert.Fail("Should have had exception thrown");
            }
            catch (CommitmentsApiModelException e)
            {
                e.Errors[0].Field.Should().Be("DeliveryModel");
                e.Errors[0].Message.Should().Be("Please select a delivery model option");
            }
        }

        [Test]
        public async Task WhenSettingDeliveryModel_AndOptionSet_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelFixture();

            fixture.ViewModel.DeliveryModel = DeliveryModel.Flexible;

            var result = await fixture.Sut.SetDeliveryModel(fixture.ViewModel) as RedirectToActionResult;
            result.ActionName.Should().Be("AddDraftApprenticeship");
        }
    }

    public class WhenSelectingDeliveryModelFixture
    {
        public CohortController Sut { get; set; }

        public string RedirectUrl;
        public Mock<IMediator> MediatorMock;
        public CreateCohortWithDraftApprenticeshipRequest Request;
        public AddDraftApprenticeshipViewModel ViewModel;

        public WhenSelectingDeliveryModelFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Build<CreateCohortWithDraftApprenticeshipRequest>().Create();
            ViewModel = fixture.Build<AddDraftApprenticeshipViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.EndMonth).Without(x => x.EndYear)
                .Without(x => x.StartDate)
                .Without(x => x.StartMonth).Without(x => x.StartYear)
                .Create();

            MediatorMock = new Mock<IMediator>();

            Sut = new CohortController(MediatorMock.Object, Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>());
        }

        public WhenSelectingDeliveryModelFixture WithDeliveryModels(List<DeliveryModel> list)
        {
            MediatorMock
                .Setup(x => x.Send(It.Is<GetProviderCourseDeliveryModelsQueryRequest>(p=>p.ProviderId == Request.ProviderId && p.CourseId == Request.CourseCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProviderCourseDeliveryModelsQueryResponse {DeliveryModels = list});
            return this;
        }

        public void VerifyReturnsRedirect(IActionResult redirectResult)
        {
            redirectResult.VerifyReturnsRedirect().Url.Equals(RedirectUrl);
        }
    }
}
