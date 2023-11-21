﻿using AutoFixture;
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
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenSelectingDeliveryModelOnAddAnotherApprenticeship
    {
        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithOnlyOneOption_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular });

            var result = await fixture.Sut.SelectDeliveryModel(fixture.Request) as RedirectToActionResult;
            result.ActionName.Should().Be("AddDraftApprenticeship");
        }

        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithMultipleOptions_ShouldRedirectToSelectDeliveryModel()
        {
            var fixture = new WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob });

            var result = await fixture.Sut.SelectDeliveryModel(fixture.Request) as ViewResult;
            result.ViewName.Should().Be("SelectDeliveryModel");
        }

        [Test]
        public async Task WhenSettingDeliveryModel_AndNoOptionSet_ShouldThrowException()
        {
            var fixture = new WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture();

            fixture.ViewModel.DeliveryModel = null;

            try
            {
                var result = await fixture.Sut.SetDeliveryModel(fixture.ViewModel);
                Assert.Fail("Should have had exception thrown");
            }
            catch (CommitmentsApiModelException e)
            {
                e.Errors[0].Field.Should().Be("DeliveryModel");
                e.Errors[0].Message.Should().Be("You must select the apprenticeship delivery model");
            }
        }

        [Test]
        public async Task WhenSettingDeliveryModel_AndOptionSet_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob});

            fixture.ViewModel.DeliveryModel = DeliveryModel.PortableFlexiJob;

            var result = await fixture.Sut.SetDeliveryModel(fixture.ViewModel) as RedirectToActionResult;
            result.ActionName.Should().Be("AddDraftApprenticeship");
        }
    }

    public class WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture
    {
        public DraftApprenticeshipController Sut { get; set; }

        public string RedirectUrl;
        public Mock<IModelMapper> ModelMapperMock;
        public Mock<IAuthorizationService> AuthorizationServiceMock;
        public SelectDeliveryModelViewModel ViewModel;
        public ReservationsAddDraftApprenticeshipRequest Request;

        public WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture()
        {
            var fixture = new Fixture();
            ViewModel = fixture.Create<SelectDeliveryModelViewModel>();
            Request = fixture.Create<ReservationsAddDraftApprenticeshipRequest>();

            ModelMapperMock = new Mock<IModelMapper>();
            AuthorizationServiceMock = new Mock<IAuthorizationService>();

            Sut = new DraftApprenticeshipController(
                Mock.Of<IMediator>(),
                Mock.Of<ICommitmentsApiClient>(),
                ModelMapperMock.Object,
                Mock.Of<IEncodingService>(),
                AuthorizationServiceMock.Object, Mock.Of<IOuterApiService>(),
                Mock.Of<IAuthenticationService>(),
                Mock.Of<ILogger<DraftApprenticeshipController>>());
        }

        public WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture WithDeliveryModels(List<DeliveryModel> list)
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
