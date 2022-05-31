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
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenSelectingDeliveryModelOnEditApprenticeship
    {
        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithOnlyOneOption_ShouldRedirectToEditDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelOnEditApprenticeshipFixture()
                .WithDraftApprenticeship()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular });

            var result = await fixture.Sut.SelectDeliveryModelForEdit(fixture.Request) as RedirectToActionResult;
            result.ActionName.Should().Be("EditDraftApprenticeship");
        }

        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithMultipleOptions_ShouldRedirectToSelectDeliveryModel()
        {
            var fixture = new WhenSelectingDeliveryModelOnEditApprenticeshipFixture()
                .WithDraftApprenticeship()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob });

            var result = await fixture.Sut.SelectDeliveryModelForEdit(fixture.Request) as ViewResult;
            result.ViewName.Should().Be("SelectDeliveryModel");
        }

        [Test]
        public async Task WhenSettingDeliveryModel_AndNoOptionSet_ShouldThrowException()
        {
            var fixture = new WhenSelectingDeliveryModelOnEditApprenticeshipFixture();

            fixture.ViewModel.DeliveryModel = null;

            try
            {
                var result = await fixture.Sut.SetDeliveryModelForEdit(fixture.ViewModel);
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
            var fixture = new WhenSelectingDeliveryModelOnEditApprenticeshipFixture()
                .WithDraftApprenticeship()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob });

            fixture.ViewModel.DeliveryModel = DeliveryModel.PortableFlexiJob;

            var result = await fixture.Sut.SetDeliveryModelForEdit(fixture.ViewModel) as RedirectToActionResult;
            result.ActionName.Should().Be("EditDraftApprenticeship");
        }
    }

    public class WhenSelectingDeliveryModelOnEditApprenticeshipFixture
    {
        public DraftApprenticeshipController Sut { get; set; }

        public string RedirectUrl;
        public Mock<IModelMapper> ModelMapperMock;
        public Mock<IAuthorizationService> AuthorizationServiceMock;
        public Mock<ITempDataDictionary> TempDataMock;
        public SelectDeliveryModelViewModel ViewModel;
        public DraftApprenticeshipRequest Request;
        public EditDraftApprenticeshipViewModel DraftApprenticeship;

        public WhenSelectingDeliveryModelOnEditApprenticeshipFixture()
        {
            var fixture = new Fixture();
            ViewModel = fixture.Create<SelectDeliveryModelViewModel>();
            Request = fixture.Create<DraftApprenticeshipRequest>();
            DraftApprenticeship = fixture.Build<EditDraftApprenticeshipViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x => x.StartDate)
                .Without(x => x.EndMonth).Without(x => x.EndYear)
                .Create();

            ModelMapperMock = new Mock<IModelMapper>();
            TempDataMock = new Mock<ITempDataDictionary>();
            AuthorizationServiceMock = new Mock<IAuthorizationService>();
            AuthorizationServiceMock.Setup(x => x.IsAuthorized(ProviderFeature.DeliveryModel)).Returns(true);

            Sut = new DraftApprenticeshipController(
                Mock.Of<IMediator>(),
                Mock.Of<ICommitmentsApiClient>(),
                ModelMapperMock.Object,
                Mock.Of<IEncodingService>(),
                AuthorizationServiceMock.Object);
            Sut.TempData = TempDataMock.Object;
        }

        public WhenSelectingDeliveryModelOnEditApprenticeshipFixture WithDeliveryModels(List<DeliveryModel> list)
        {
            ModelMapperMock.Setup(x => x.Map<SelectDeliveryModelViewModel>(It.IsAny<EditDraftApprenticeshipViewModel>()))
                .ReturnsAsync(new SelectDeliveryModelViewModel { DeliveryModels = list.ToArray() });
            return this;
        }

        public WhenSelectingDeliveryModelOnEditApprenticeshipFixture WithDraftApprenticeship()
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
