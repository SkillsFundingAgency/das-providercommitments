using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

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
            PostConfirmEmployerFixture.VerifySelectLearnerRecordRedirect(result);
        }

        [Test]
        public async Task PostConfirmEmployerViewModel_WithValidModel_WithConfirmTrue_withNoDeclaredStandards_ShouldRedirectToNoDeclaredStandardsPage()
        {
            var fixture = new PostConfirmEmployerFixture()
                .WithConfirmTrue()
                .WithHasNoDeclaredStandards(true);

            var result = await fixture.Act();
            PostConfirmEmployerFixture.VerifyNoDeclaredStandardsRedirect(result);
        }
    }

    public class PostConfirmEmployerFixture
    {
        private readonly ConfirmEmployerRedirectModel _hasDeclaredStandardsViewModel;
        private readonly ConfirmEmployerViewModel _viewModel;

        private readonly CohortController _sut;
        private readonly string _redirectUrl;

        public PostConfirmEmployerFixture()
        {
            var fixture = new Fixture();
            const long providerId = 123;
            
            _viewModel = new ConfirmEmployerViewModel { ProviderId = providerId, EmployerAccountLegalEntityPublicHashedId = "XYZ" };
            var commitmentApiClient = new Mock<ICommitmentsApiClient>();

            var emptyCohortRequest = fixture.Create<CreateEmptyCohortRequest>();
            _hasDeclaredStandardsViewModel = fixture.Create<ConfirmEmployerRedirectModel>();
            var emptyCohortResponse = fixture.Create<CreateCohortResponse>();

            var mockModelMapper = new Mock<IModelMapper>();
            mockModelMapper
                .Setup(x => x.Map<CreateEmptyCohortRequest>(_viewModel))
                .ReturnsAsync(emptyCohortRequest);

            mockModelMapper
                .Setup(x => x.Map<ConfirmEmployerRedirectModel>(_viewModel))
                .ReturnsAsync(_hasDeclaredStandardsViewModel);

            commitmentApiClient
                .Setup(x => x.CreateCohort(emptyCohortRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyCohortResponse);

            _redirectUrl = $"{providerId}/reservations/{_viewModel.EmployerAccountLegalEntityPublicHashedId}/select";
            var linkGenerator = new Mock<ILinkGenerator>();
            linkGenerator.Setup(x => x.ReservationsLink(_redirectUrl)).Returns(_redirectUrl);

            _sut = new CohortController(Mock.Of<IMediator>(), mockModelMapper.Object, linkGenerator.Object, commitmentApiClient.Object, 
                         Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
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
            var equals = redirectResult.VerifyReturnsRedirect().Url.Equals(_redirectUrl);
        }

        public static void VerifyNoDeclaredStandardsRedirect(IActionResult redirectResult)
        {
            redirectResult.VerifyReturnsRedirectToActionResult().WithActionName("NoDeclaredStandards");
        }

        public static void VerifySelectLearnerRecordRedirect(IActionResult redirectResult)
        {
            redirectResult.VerifyReturnsRedirectToActionResult().WithActionName("SelectLearnerRecord");
        }

        public async Task<IActionResult> Act() => await _sut.ConfirmEmployer(_viewModel);
    }
}
