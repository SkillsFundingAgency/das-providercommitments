using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
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
        public async Task WithValidModel_RedirectToSent()
        {
            var result = await _fixture.Act();
            result.VerifyReturnsRedirectToRouteResult().WithRouteName(RouteNames.ApprenticeSent);
        }

        internal class WhenPostingConfirmFixture
        {
            private readonly ApprenticeController _sut;
            private readonly ConfirmViewModel _viewModel;
            private readonly ChangeOfPartyRequestRequest _mapperResult;
            private readonly Mock<ICommitmentsApiClient> _apiClient;
            private readonly Mock<IModelMapper> _modelMapper;

            public WhenPostingConfirmFixture()
            {
                _apiClient = new Mock<ICommitmentsApiClient>();
                _apiClient.Setup(x => x.ChangeOfPartyRequest(It.IsAny<long>(), It.IsAny<ChangeOfPartyRequestRequest>(),
                    It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

                _mapperResult = new ChangeOfPartyRequestRequest();

                _modelMapper = new Mock<IModelMapper>();
                _modelMapper.Setup(x => x.Map<ChangeOfPartyRequestRequest>(It.IsAny<ConfirmViewModel>()))
                    .ReturnsAsync(_mapperResult);

                _viewModel = new ConfirmViewModel
                {
                    ApprenticeshipId = 123,
                    ApprenticeshipHashedId = "DF34WG2",
                    ProviderId = 2342,
                    EmployerAccountLegalEntityPublicHashedId = "DFF41G",
                    NewStartDate = new MonthYearModel("62020")
                };

                _sut = new ApprenticeController(_modelMapper.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), _apiClient.Object);
            }

            public Task<IActionResult> Act() => _sut.Confirm(_viewModel);

            public void VerifyChangeOfPartyRequestCreated()
            {
                _apiClient.Verify(x => x.ChangeOfPartyRequest(It.Is<long>(id => id == _viewModel.ApprenticeshipId),
                    It.Is<ChangeOfPartyRequestRequest>(r => r == _mapperResult),
                    It.IsAny<CancellationToken>()));
            }
        }
    }
}
