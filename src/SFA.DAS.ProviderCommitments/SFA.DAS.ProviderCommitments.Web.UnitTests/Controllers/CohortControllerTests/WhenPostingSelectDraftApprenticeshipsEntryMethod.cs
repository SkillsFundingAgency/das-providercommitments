using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
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
        public CohortController Sut { get; set; }

        private readonly SelectDraftApprenticeshipsEntryMethodViewModel _viewModel;
        public readonly long ProviderId = 123;

        public WhenPostingSelectDraftApprenticeshipsEntryMethodFixture()
        {
            _viewModel = new SelectDraftApprenticeshipsEntryMethodViewModel { ProviderId = ProviderId };

            Sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
                        Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>(),Mock.Of<IOuterApiService>(), Mock.Of<IAuthenticationService>());
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

        public IActionResult Act() => Sut.SelectDraftApprenticeshipsEntryMethod(_viewModel);
    }
}