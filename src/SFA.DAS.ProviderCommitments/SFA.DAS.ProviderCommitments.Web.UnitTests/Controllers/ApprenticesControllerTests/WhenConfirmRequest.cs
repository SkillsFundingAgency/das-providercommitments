using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenConfirmRequest
    {
        [Test]
        public async Task GetThenCallsPriceViewModelMapper()
        {
            var fixture = new WhenConfirmRequestFixture();

            await fixture.Sut.Confirm(fixture.ChangeOfEmployerRequest);

            fixture.VerifyConfirmViewModelMapperWasCalled();
        }

        [Test]
        public async Task GetThenReturnsView()
        {
            var fixture = new WhenConfirmRequestFixture();

            var result = await fixture.Sut.Confirm(fixture.ChangeOfEmployerRequest) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(ConfirmViewModel), result.Model.GetType());
        }
    }

    public class WhenConfirmRequestFixture
    {
        public ApprenticeController Sut { get; set; }
        public ConfirmRequest ChangeOfEmployerRequest { get; set; }
        public ConfirmViewModel ChangeOfEmployerViewModel { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly Fixture _fixture;

        public WhenConfirmRequestFixture()
        {
            _fixture = new Fixture();
            ChangeOfEmployerRequest = _fixture.Build<ConfirmRequest>().Create();
            ChangeOfEmployerViewModel = _fixture.Build<ConfirmViewModel>()
                .With(x => x.NewStartDate, "042020")
                .Without(x => x.NewEmploymentEndDate)
                .Create();

            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(x => x.Map<ConfirmViewModel>(It.IsAny<ConfirmRequest>()))
                .ReturnsAsync(ChangeOfEmployerViewModel);

            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        public void VerifyConfirmViewModelMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ConfirmViewModel>(ChangeOfEmployerRequest));
        }
    }
}
