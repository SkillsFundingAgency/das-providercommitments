﻿using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingChooseCohort
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new WhenGettingChooseCohortFixture();

            await fixture.Sut.ChooseCohort(fixture.Request);

            fixture.ModelMapperMock.Verify(x => x.Map<ChooseCohortViewModel>(fixture.Request));
        }

        [Test]
        public async Task ThenReturnsChooseCohortsViewModel()
        {
            var fixture = new WhenGettingChooseCohortFixture();

            var result = await fixture.Sut.ChooseCohort(fixture.Request) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model.GetType(), Is.EqualTo(typeof(ChooseCohortViewModel)));
        }
    }

    public class WhenGettingChooseCohortFixture
    {
        public CohortController Sut { get; }
        public ChooseCohortByProviderRequest Request { get; }
        public Mock<IModelMapper> ModelMapperMock { get; }

        public WhenGettingChooseCohortFixture()
        {
            Request = new ChooseCohortByProviderRequest();
            ModelMapperMock = new Mock<IModelMapper>();
            var chooseCohortViewModel = new ChooseCohortViewModel();

            ModelMapperMock.Setup(x => x.Map<ChooseCohortViewModel>(Request)).ReturnsAsync(chooseCohortViewModel);

            Sut = new CohortController(Mock.Of<IMediator>(), ModelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                    Mock.Of<IEncodingService>(),  Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public async Task<IActionResult> Act() => await Sut.ChooseCohort(Request);
    }
}
