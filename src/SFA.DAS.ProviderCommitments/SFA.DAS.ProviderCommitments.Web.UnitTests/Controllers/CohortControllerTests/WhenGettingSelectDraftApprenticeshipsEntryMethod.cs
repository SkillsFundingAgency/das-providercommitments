using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingSelectDraftApprenticeshipsEntryMethod
    {
        [Test]
        public void ThenReturnsView()
        {
            var fixture = new WhenGettingSelectDraftApprenticeshipsEntryMethodFixture();

            var result = fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public void ThenProviderIdIsMapped()
        {
            var fixture = new WhenGettingSelectAddDraftApprenticeshipJourneyFixture();

            var viewResult = fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<SelectAddDraftApprenticeshipJourneyViewModel>();

            Assert.AreEqual(fixture.ProviderId, model.ProviderId);
        }
    }

    public class WhenGettingSelectDraftApprenticeshipsEntryMethodFixture
    {
        public CohortController Sut { get; set; }

        private readonly SelectAddDraftApprenticeshipJourneyRequest _request;
        public readonly long ProviderId = 123;

        public WhenGettingSelectDraftApprenticeshipsEntryMethodFixture()
        {
            _request = new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = ProviderId };
            Sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IEncodingService>());
        }

        public IActionResult Act() => Sut.SelectDraftApprenticeshipsEntryMethod(_request);
    }
}