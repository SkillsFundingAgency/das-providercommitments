using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenSelectingDeliveryModel
    {
        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithOnlyOneOption_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular });

            var result = await fixture.Sut.SelectDeliveryModel(fixture.Request) as RedirectToActionResult;
            result.ActionName.Should().Be("AddDraftApprenticeship");
        }

        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithMultipleOptions_ShouldRedirectToSelectDeliveryModel()
        {
            var fixture = new WhenSelectingDeliveryModelFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob });

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
            var fixture = new WhenSelectingDeliveryModelFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob});

            fixture.ViewModel.DeliveryModel = DeliveryModel.PortableFlexiJob;

            var result = await fixture.Sut.SetDeliveryModel(fixture.ViewModel) as RedirectToActionResult;
            result.ActionName.Should().Be("AddDraftApprenticeship");
        }
    }

    public class WhenSelectingDeliveryModelFixture
    {
        public DraftApprenticeshipController Sut { get; set; }

        public string RedirectUrl;
        public Mock<IModelMapper> ModelMapperMock;
        public Mock<IAuthorizationService> AuthorizationServiceMock;
        public SelectDeliveryModelViewModel ViewModel;
        public ReservationsAddDraftApprenticeshipRequest Request;

        public WhenSelectingDeliveryModelFixture()
        {
            var fixture = new Fixture();
            ViewModel = fixture.Create<SelectDeliveryModelViewModel>();
            Request = fixture.Create<ReservationsAddDraftApprenticeshipRequest>();

            ModelMapperMock = new Mock<IModelMapper>();
            AuthorizationServiceMock = new Mock<IAuthorizationService>();
            AuthorizationServiceMock.Setup(x => x.IsAuthorized(ProviderFeature.DeliveryModel)).Returns(true);

            Sut = new DraftApprenticeshipController(Mock.Of<IMediator>(), Mock.Of<ICommitmentsApiClient>(), ModelMapperMock.Object, Mock.Of<IEncodingService>(), AuthorizationServiceMock.Object);
        }

        public WhenSelectingDeliveryModelFixture WithDeliveryModels(List<DeliveryModel> list)
        {
            ModelMapperMock.Setup(x => x.Map<SelectDeliveryModelViewModel>(Request))
                .ReturnsAsync(new SelectDeliveryModelViewModel {DeliveryModels = list.ToArray()});
            return this;
        }

        public void VerifyReturnsRedirect(IActionResult redirectResult)
        {
            redirectResult.VerifyReturnsRedirect().Url.Equals(RedirectUrl);
        }
    }
}
