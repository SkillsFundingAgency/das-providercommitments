using System.ComponentModel.Design;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using AutoFixture;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderUrlHelper;

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
        public async Task PostThenCallsConfirmRequestMapper()
        {
            var fixture = new WhenAddingNewPriceFixture();

            await fixture.Sut.Price(fixture.PriceViewModel);

            fixture.VerifyConfirmRequestMapperWasCalled();
        }

        [Test]
        public async Task PostThenReturnsARedirectResult()
        {
            var fixture = new WhenAddingNewPriceFixture();

            var result = await fixture.Sut.Price(fixture.PriceViewModel) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(RouteNames.ApprenticeConfirm, result.RouteName);
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

            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        public void VerifyPriceViewMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<PriceViewModel>(PriceRequest));
        }

        public void VerifyConfirmRequestMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ConfirmRequest>(PriceViewModel));
        }
    }
}
