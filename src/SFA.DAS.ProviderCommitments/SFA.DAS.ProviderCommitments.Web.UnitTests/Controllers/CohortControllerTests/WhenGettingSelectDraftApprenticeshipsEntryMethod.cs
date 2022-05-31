using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;

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
        public async Task ThenProviderIdIsMapped()
        {
            var fixture = new WhenGettingSelectAddDraftApprenticeshipJourneyFixture();

            var viewResult = await fixture.ActAsync();

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
            Sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(), Mock.Of<RecognitionOfPriorLearningConfiguration>());
        }

        public IActionResult Act() => Sut.SelectDraftApprenticeshipsEntryMethod(_request);
    }
}