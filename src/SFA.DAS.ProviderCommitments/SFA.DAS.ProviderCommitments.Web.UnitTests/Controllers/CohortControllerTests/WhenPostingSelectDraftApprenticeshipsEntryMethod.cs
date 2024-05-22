using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenPostingSelectDraftApprenticeshipsEntryMethod
    {
        [Test]
        public void Then_RedirectTo_SelectJourney_When_Selected_Option_Is_Manual()
        {
            var fixture = new WhenPostingSelectDraftApprenticeshipsEntryMethodFixture();

            var result = fixture.Manual().Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName("SelectAddDraftApprenticeshipJourney");
        }

        [Test]
        public void Then_RedirectTo_FileUploadInform_When_Selected_Option_Is_BulkCsv()
        {
            var fixture = new WhenPostingSelectDraftApprenticeshipsEntryMethodFixture();

            var result = fixture.BulkCsv().Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadInform");
        }
    }

    public class WhenPostingSelectDraftApprenticeshipsEntryMethodFixture
    {
        private readonly CohortController _sut;
        private readonly SelectDraftApprenticeshipsEntryMethodViewModel _viewModel;
        private const long ProviderId = 123;

        public WhenPostingSelectDraftApprenticeshipsEntryMethodFixture()
        {
            _viewModel = new SelectDraftApprenticeshipsEntryMethodViewModel { ProviderId = ProviderId };

            _sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
                        Mock.Of<IEncodingService>(),Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public WhenPostingSelectDraftApprenticeshipsEntryMethodFixture Manual()
        {
            _viewModel.Selection = AddDraftApprenticeshipEntryMethodOptions.Manual;
            return this;
        }

        public WhenPostingSelectDraftApprenticeshipsEntryMethodFixture BulkCsv()
        {
            _viewModel.Selection = AddDraftApprenticeshipEntryMethodOptions.BulkCsv;
            return this;
        }

        public IActionResult Act() => _sut.SelectDraftApprenticeshipsEntryMethod(_viewModel);
    }
}