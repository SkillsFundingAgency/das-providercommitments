using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenPostingConfirm
    {
        private WhenPostingConfirmFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenPostingConfirmFixture();
        }

        [Test]
        public async Task WithValidModel_ChangeOfPartyRequest_Is_Created()
        {
            await _fixture.Act();
            _fixture.VerifyChangeOfPartyRequestCreated();
        }

        [Test]
        public async Task WithValidModel_NewEmployerName_Is_Stored_In_TempData()
        {
            await _fixture.Act();

            _fixture.VerifyNewEmployerNameIsStoredInTempData();
        }

        [Test]
        public async Task WithValidModel_RedirectToSent()
        {
            var result = await _fixture.Act();
            result.VerifyReturnsRedirectToRouteResult().WithRouteName(RouteNames.ApprenticeSent);
        }

        private class WhenPostingConfirmFixture
        {
            private readonly ApprenticeController _sut;
            private readonly ConfirmViewModel _viewModel;
            private readonly Mock<IModelMapper> _modelMapper;

            public WhenPostingConfirmFixture()
            {
                var apiClient = new Mock<ICommitmentsApiClient>();
                apiClient.Setup(x => x.CreateChangeOfPartyRequest(It.IsAny<long>(), It.IsAny<CreateChangeOfPartyRequestRequest>(),
                    It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

                var mapperResult = new SentRequest();

                _modelMapper = new Mock<IModelMapper>();
                _modelMapper.Setup(x => x.Map<SentRequest>(It.IsAny<ConfirmViewModel>()))
                    .ReturnsAsync(mapperResult);

                _viewModel = new ConfirmViewModel
                {
                    ApprenticeshipId = 123,
                    ApprenticeshipHashedId = "DF34WG2",
                    ProviderId = 2342,
                    AccountLegalEntityPublicHashedId = "DFF41G",
                    NewStartDate = "62020",
                    NewEmployerName = "TestEmployerName"
                };

                var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

                _sut = new ApprenticeController(_modelMapper.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), apiClient.Object, Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>(), Mock.Of<ILogger<ApprenticeController>>());

                _sut.TempData = tempData;
            }

            public Task<IActionResult> Act() => _sut.Confirm(_viewModel);

            public void VerifyChangeOfPartyRequestCreated()
            {
                _modelMapper.Verify(x => x.Map<SentRequest>(It.IsAny<ConfirmViewModel>()));
            }

            public void VerifyNewEmployerNameIsStoredInTempData()
            {
                var newEmployerName = _sut.TempData[nameof(ConfirmViewModel.NewEmployerName)] as string;
                Assert.That(newEmployerName, Is.Not.Null);
                Assert.That(newEmployerName, Is.EqualTo(_viewModel.NewEmployerName));
            }
        }
    }
}
