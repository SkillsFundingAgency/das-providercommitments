using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using System.Threading;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.Authorization.Services;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Authentication;

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
        public async Task PostConfirmEmployerViewModel_WithValidModel_WithConfirmTrue_ShouldCreateCohortAndRedirectToCohortDetailsPage()
        {
            var fixture = new PostConfirmEmployerFixture()
                .WithConfirmTrue()
                .WithHasNoDeclaredStandards(false);

            var result = await fixture.Act();
            fixture.VerifyReturnsRedirect(result);
        }

        [Test]
        public async Task PostConfirmEmployerViewModel_WithValidModel_WithConfirmTrue_withNoDeclaredStandards_ShouldRedirectToNoDeclaredStandardsPage()
        {
            var fixture = new PostConfirmEmployerFixture()
                .WithConfirmTrue()
                .WithHasNoDeclaredStandards(true);

            var result = await fixture.Act();
            fixture.VerifyNoDeclaredStandardsRedirect(result);
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
        private readonly ConfirmEmployerRedirectModel _hasDeclaredStandardsViewModel;
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
            _hasDeclaredStandardsViewModel = fixture.Create<ConfirmEmployerRedirectModel>();
            _emptyCohortResponse = fixture.Create<CommitmentsV2.Api.Types.Responses.CreateCohortResponse>();

            _mockModelMapper = new Mock<IModelMapper>();
            _mockModelMapper
                .Setup(x => x.Map<CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>(_viewModel))
                .ReturnsAsync(_emptyCohortRequest);

            _mockModelMapper
                .Setup(x => x.Map<ConfirmEmployerRedirectModel>(_viewModel))
                .ReturnsAsync(_hasDeclaredStandardsViewModel);

            _commitmentApiClient
                .Setup(x => x.CreateCohort(_emptyCohortRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_emptyCohortResponse);

            RedirectUrl = $"{_providerId}/reservations/{_viewModel.EmployerAccountLegalEntityPublicHashedId}/select";
            _linkGenerator = new Mock<ILinkGenerator>();
            _linkGenerator.Setup(x => x.ReservationsLink(RedirectUrl)).Returns(RedirectUrl);

            Sut = new CohortController(Mock.Of<IMediator>(), _mockModelMapper.Object, _linkGenerator.Object, _commitmentApiClient.Object, 
                        Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(), Mock.Of<IAuthenticationService>());
        }

        public PostConfirmEmployerFixture WithHasNoDeclaredStandards(bool hasDeclaredStandards)
        {
            _hasDeclaredStandardsViewModel.HasNoDeclaredStandards = hasDeclaredStandards;
            return this;
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

        public void VerifyReturnsRedirect(IActionResult redirectResult)
        {
            redirectResult.VerifyReturnsRedirect().Url.Equals(RedirectUrl);
        }
        public void VerifyNoDeclaredStandardsRedirect(IActionResult redirectResult)
        {
            redirectResult.VerifyReturnsRedirectToActionResult().WithActionName("NoDeclaredStandards");
        }


        public async Task<IActionResult> Act() => await Sut.ConfirmEmployer(_viewModel);
    }
}
