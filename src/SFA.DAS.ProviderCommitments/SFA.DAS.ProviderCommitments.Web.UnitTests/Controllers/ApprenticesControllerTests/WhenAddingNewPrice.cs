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
        public async Task GetThenCallsChangePriceViewModelMapper()
        {
            var fixture = new WhenAddingNewPriceFixture();

            await fixture.Sut.ChangePrice(fixture.ChangePriceRequest);

            fixture.VerifyChangePriceViewMapperWasCalled();
        }

        [Test]
        public async Task GetThenReturnsView()
        {
            var fixture = new WhenAddingNewPriceFixture();

            var result = await fixture.Sut.ChangePrice(fixture.ChangePriceRequest) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(ChangePriceViewModel), result.Model.GetType());
        }

        [Test]
        public async Task PostThenCallsChangeOfEmployerRequestModelMapper()
        {
            var fixture = new WhenAddingNewPriceFixture();

            await fixture.Sut.ChangePrice(fixture.ChangePriceViewModel);

            fixture.VerifyChangeOfEmployerMapperWasCalled();
        }

        [Test]
        public async Task PostThenReturnsARedirectResult()
        {
            var fixture = new WhenAddingNewPriceFixture();

            var result = await fixture.Sut.ChangePrice(fixture.ChangePriceViewModel) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(RouteNames.ApprenticeConfirmChangeOfEmployer, result.RouteName);
        }
    }

    public class WhenAddingNewPriceFixture
    {
        public ApprenticeController Sut { get; set; }
        public ChangePriceRequest ChangePriceRequest { get; set; }
        public ChangePriceViewModel ChangePriceViewModel { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly Fixture _fixture;

        public WhenAddingNewPriceFixture()
        {
            _fixture = new Fixture();
            ChangePriceRequest = _fixture.Create<ChangePriceRequest>();
            ChangePriceViewModel = _fixture.Create<ChangePriceViewModel>();

            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(x => x.Map<ChangePriceViewModel>(It.IsAny<ChangePriceRequest>()))
                .ReturnsAsync(ChangePriceViewModel);

            Sut = new ApprenticeController(_modelMapperMock.Object);
        }

        public void VerifyChangePriceViewMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ChangePriceViewModel>(ChangePriceRequest));
        }

        public void VerifyChangeOfEmployerMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ChangeOfEmployerRequest>(ChangePriceViewModel));
        }
    }
}
