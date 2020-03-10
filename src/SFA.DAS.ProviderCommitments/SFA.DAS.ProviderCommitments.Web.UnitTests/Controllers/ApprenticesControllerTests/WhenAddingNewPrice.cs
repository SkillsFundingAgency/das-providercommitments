using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using AutoFixture;
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
        public async Task PostThenCallsChangeOfEmployerRequestModelMapper()
        {
            var fixture = new WhenAddingNewPriceFixture();

            await fixture.Sut.Price(fixture.PriceViewModel);

            fixture.VerifyChangeOfEmployerMapperWasCalled();
        }

        [Test]
        public async Task PostThenReturnsARedirectResult()
        {
            var fixture = new WhenAddingNewPriceFixture();

            var result = await fixture.Sut.Price(fixture.PriceViewModel) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(RouteNames.ApprenticeConfirmChangeOfEmployer, result.RouteName);
        }
    }

    public class WhenAddingNewPriceFixture
    {
        public ApprenticeController Sut { get; set; }
        public PriceRequest PriceRequest { get; set; }
        public PriceViewModel PriceViewModel { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly Fixture _fixture;

        public WhenAddingNewPriceFixture()
        {
            _fixture = new Fixture();
            PriceRequest = _fixture.Create<PriceRequest>();
            PriceViewModel = _fixture.Create<PriceViewModel>();

            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(x => x.Map<PriceViewModel>(It.IsAny<PriceRequest>()))
                .ReturnsAsync(PriceViewModel);

            Sut = new ApprenticeController(_modelMapperMock.Object);
        }

        public void VerifyPriceViewMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<PriceViewModel>(PriceRequest));
        }

        public void VerifyChangeOfEmployerMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ChangeOfEmployerRequest>(PriceViewModel));
        }
    }
}
