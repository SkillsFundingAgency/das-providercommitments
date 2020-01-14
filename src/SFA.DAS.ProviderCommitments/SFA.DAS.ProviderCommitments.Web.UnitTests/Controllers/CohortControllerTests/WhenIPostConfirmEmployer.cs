using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;
using System.Threading;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenIPostConfirmEmployer
    {
        [Test]
        public async Task PostConfirmEmployerViewModel_WithValidModel_WithConfirmFalse_ShouldRedirectToSelectEmployer()
        {
            var fixture = new PostConfirmEmployerFixture()
                .WithConfirmFalse();

            var result = await fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("SelectEmployer");
        }

        [Test]
        public async Task PostConfirmEmployerViewModel_WithValidModel_WithConfirmTrue_ThenRequestIsMapped()
        {
            var fixture = new PostConfirmEmployerFixture()
                .WithConfirmTrue();
            await fixture.Act();
            fixture.VerifyMapperIsCalled();
        }

        [Test]
        public async Task PostConfirmEmployerViewModel_WithValidModel_WithConfirmTrue_ShouldCreateCohortAndRedirectToCohortDetailsPage()
        {
            var fixture = new PostConfirmEmployerFixture()
                .WithConfirmTrue();

            var result = await fixture.Act();
            fixture.VerifyCohortCreated();
            result.VerifyReturnsRedirect().WithUrl(fixture.RedirectUrl);
        }
    }

    public class PostConfirmEmployerFixture
    {
        public CohortController Sut { get; set; }

        public string RedirectUrl;
        private readonly Mock<ILinkGenerator> _linkGenerator;
        private readonly Mock<IModelMapper> _mockModelMapper;
        private readonly Mock<ICommitmentsApiClient> _commitmentApiClient;
        private readonly CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest _emptyCohortRequest;
        private readonly CommitmentsV2.Api.Types.Responses.CreateCohortResponse _emptyCohortResponse;
        private readonly ConfirmEmployerViewModel _viewModel;
        private readonly long _providerId;

        public PostConfirmEmployerFixture()
        {
            var fixture = new Fixture();
            _providerId = 123;
            _viewModel = new ConfirmEmployerViewModel { ProviderId = _providerId, EmployerAccountLegalEntityPublicHashedId = "XYZ" };
            _commitmentApiClient = new Mock<ICommitmentsApiClient>();

            _emptyCohortRequest = fixture.Create<CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>();
            _emptyCohortResponse = fixture.Create<CommitmentsV2.Api.Types.Responses.CreateCohortResponse>();

            _mockModelMapper = new Mock<IModelMapper>();
            _mockModelMapper
                .Setup(x => x.Map<CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>(_viewModel))
                .ReturnsAsync(_emptyCohortRequest);

            _commitmentApiClient
                .Setup(x => x.CreateCohort(_emptyCohortRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_emptyCohortResponse);

            RedirectUrl = $"{_viewModel.ProviderId}/apprentices/{_emptyCohortResponse.CohortReference}/Details";
            _linkGenerator = new Mock<ILinkGenerator>();
            _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(RedirectUrl)).Returns(RedirectUrl);

            Sut = new CohortController(Mock.Of<IMediator>(), _mockModelMapper.Object, _linkGenerator.Object, _commitmentApiClient.Object);
        }

        public PostConfirmEmployerFixture WithConfirmFalse()
        {
            _viewModel.Confirm = false;
            return this;
        }

        public PostConfirmEmployerFixture WithConfirmTrue()
        {
            _viewModel.Confirm = true;
            return this;
        }

        public PostConfirmEmployerFixture VerifyMapperIsCalled()
        {
            _mockModelMapper.Verify(x => x.Map<CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>(_viewModel));
            return this;
        }

        public PostConfirmEmployerFixture VerifyCohortCreated()
        {
            _commitmentApiClient.Verify(x => x.CreateCohort(_emptyCohortRequest, It.IsAny<CancellationToken>()), Times.Once);
            return this;
        }

        public async Task<IActionResult> Act() => await Sut.ConfirmEmployer(_viewModel);
    }
}
