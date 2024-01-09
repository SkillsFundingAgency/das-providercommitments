using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenAddingNewPrice
    {
        [Test]
        public async Task GetThenCallsPriceViewModelMapper()
        {
            var fixture = new WhenAddingNewPriceFixture();

            await fixture.Sut.Price(fixture.PriceRequest);

            fixture.VerifyPriceViewMapperWasCalled();
        }

        [Test]
        public async Task GetThenReturnsView()
        {
            var fixture = new WhenAddingNewPriceFixture();

            var result = await fixture.Sut.Price(fixture.PriceRequest) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(PriceViewModel), result.Model.GetType());
        }

        [Test]
        [TestCase(ApprenticeshipStatus.Stopped)]
        [TestCase(ApprenticeshipStatus.Live)]
        [TestCase(ApprenticeshipStatus.Completed)]
        [TestCase(ApprenticeshipStatus.Paused)]
        [TestCase(ApprenticeshipStatus.WaitingToStart)]
        public async Task PostThenCallsConfirmRequestMapper(ApprenticeshipStatus status)
        {
            var fixture = new WhenAddingNewPriceFixture { PriceViewModel = { ApprenticeshipStatus = status } };

            await fixture.Sut.Price(fixture.PriceViewModel);

            if (status == ApprenticeshipStatus.Stopped)
            {
                fixture.VerifyConfirmRequestMapperWasCalled();
            }
            else
            {
                fixture.VerifyChangeOfEmployerOverlapAlertRequestWasCalled();
            }
        }

        [Test]
        [TestCase(ApprenticeshipStatus.Stopped)]
        [TestCase(ApprenticeshipStatus.Live)]
        [TestCase(ApprenticeshipStatus.Completed)]
        [TestCase(ApprenticeshipStatus.Paused)]
        [TestCase(ApprenticeshipStatus.WaitingToStart)]
        public async Task PostThenReturnsARedirectResult(ApprenticeshipStatus status)
        {
            var fixture = new WhenAddingNewPriceFixture { PriceViewModel = { ApprenticeshipStatus = status } };

            var result = await fixture.Sut.Price(fixture.PriceViewModel) as RedirectToRouteResult;

            Assert.NotNull(result);

            if (status == ApprenticeshipStatus.Stopped)
            {
                Assert.AreEqual(RouteNames.ApprenticeConfirm, result.RouteName);
            }
            else
            {
                Assert.AreEqual(RouteNames.ChangeEmployerOverlapAlert, result.RouteName);
            }
        }
    }

    public class WhenAddingNewPriceFixture
    {
        public ApprenticeController Sut { get; set; }
        public PriceRequest PriceRequest { get; set; }
        public PriceViewModel PriceViewModel { get; set; }
        public ConfirmRequest ConfirmRequest { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly Fixture _fixture;

        public WhenAddingNewPriceFixture()
        {
            _fixture = new Fixture();
            PriceRequest = _fixture.Create<PriceRequest>();
            PriceViewModel = _fixture.Create<PriceViewModel>();
            ConfirmRequest = _fixture.Create<ConfirmRequest>();

            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(x => x.Map<PriceViewModel>(It.IsAny<PriceRequest>()))
                .ReturnsAsync(PriceViewModel);
            _modelMapperMock.Setup(x => x.Map<ConfirmRequest>(It.IsAny<PriceViewModel>()))
                .ReturnsAsync(ConfirmRequest);

            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(),
                Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>());
        }

        public void VerifyPriceViewMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<PriceViewModel>(PriceRequest));
        }

        public void VerifyConfirmRequestMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ConfirmRequest>(PriceViewModel));
        }

        public void VerifyChangeOfEmployerOverlapAlertRequestWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ChangeOfEmployerOverlapAlertRequest>(PriceViewModel));
        }
    }
}