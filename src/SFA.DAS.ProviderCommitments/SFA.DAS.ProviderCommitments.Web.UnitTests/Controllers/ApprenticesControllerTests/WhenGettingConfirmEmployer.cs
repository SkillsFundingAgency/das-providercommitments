using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticeControllerTests
{
    [TestFixture]
    public class WhenGettingConfirmEmployer
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new GetConfirmEmployerFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new GetConfirmEmployerFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel().WithModel<ConfirmEmployerViewModel>();
        }
    }

    public class GetConfirmEmployerFixture
    {
        public ApprenticeController Sut { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly ConfirmEmployerViewModel _viewModel;
        private readonly ConfirmEmployerRequest _request;
        private readonly long _providerId;

        public GetConfirmEmployerFixture()
        {
            var fixture = new Fixture();
            _request = new ConfirmEmployerRequest { ProviderId = _providerId, EmployerAccountLegalEntityPublicHashedId = "XYZ" };
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = fixture.Create<ConfirmEmployerViewModel>();
            _providerId = 123;

            _modelMapperMock
                .Setup(x => x.Map<ConfirmEmployerViewModel>(_request))
                .ReturnsAsync(_viewModel);
            
            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        public GetConfirmEmployerFixture WithModelStateErrors()
        {
            Sut.ControllerContext.ModelState.AddModelError("TestError", "Test Error");
            return this;
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ConfirmEmployerViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.ConfirmEmployer(_request);
    }
}
