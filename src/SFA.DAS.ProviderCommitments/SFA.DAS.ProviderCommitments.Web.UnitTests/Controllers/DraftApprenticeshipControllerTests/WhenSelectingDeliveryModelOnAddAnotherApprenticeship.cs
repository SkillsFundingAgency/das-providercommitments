using System.Collections.Generic;
using FluentAssertions;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenSelectingDeliveryModelOnAddAnotherApprenticeship
    {
        [Test]
        public async Task
            GettingDeliveryModel_ForProviderAndCourse_WithOnlyOneOption_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular });

            var result = await fixture.Sut.SelectDeliveryModel(fixture.Request) as RedirectToActionResult;
            result.ActionName.Should().Be("AddDraftApprenticeship");
        }

        [Test]
        public async Task
            GettingDeliveryModel_ForProviderAndCourse_WithMultipleOptions_ShouldRedirectToSelectDeliveryModel()
        {
            var fixture = new WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob });

            var result = await fixture.Sut.SelectDeliveryModel(fixture.Request) as ViewResult;
            result.ViewName.Should().Be("SelectDeliveryModel");
        }

        [Test]
        public async Task WhenSettingDeliveryModel_AndNoOptionSet_ShouldThrowException()
        {
            var fixture = new WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture
            {
                ViewModel =
                {
                    DeliveryModel = null
                }
            };

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
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob });

            fixture.ViewModel.DeliveryModel = DeliveryModel.PortableFlexiJob;

            var result = await fixture.Sut.SetDeliveryModel(fixture.ViewModel) as RedirectToActionResult;
            result.ActionName.Should().Be("AddDraftApprenticeship");
        }
    }

    public class WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture
    {
        private readonly Mock<IModelMapper> _modelMapperMock;

        public DraftApprenticeshipController Sut { get; }
        public SelectDeliveryModelViewModel ViewModel { get; }
        public ReservationsAddDraftApprenticeshipRequest Request { get; }

        public WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture()
        {
            var fixture = new Fixture();
            ViewModel = fixture.Create<SelectDeliveryModelViewModel>();
            Request = fixture.Create<ReservationsAddDraftApprenticeshipRequest>();

            _modelMapperMock = new Mock<IModelMapper>();
            var authorizationServiceMock = new Mock<IAuthorizationService>();

            Sut = new DraftApprenticeshipController(
                Mock.Of<IMediator>(),
                Mock.Of<ICommitmentsApiClient>(),
                _modelMapperMock.Object,
                Mock.Of<IEncodingService>(),
                authorizationServiceMock.Object, Mock.Of<IOuterApiService>(),
                Mock.Of<IAuthenticationService>());
        }

        public WhenSelectingDeliveryModelOnAddAnotherApprenticeshipFixture WithDeliveryModels(List<DeliveryModel> list)
        {
            _modelMapperMock.Setup(x => x.Map<SelectDeliveryModelViewModel>(Request))
                .ReturnsAsync(new SelectDeliveryModelViewModel { DeliveryModels = list.ToArray() });
            return this;
        }
    }
}