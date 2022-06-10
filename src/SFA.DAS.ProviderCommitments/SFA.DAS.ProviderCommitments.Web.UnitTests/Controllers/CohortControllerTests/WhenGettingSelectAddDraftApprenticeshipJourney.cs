using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingSelectAddDraftApprenticeshipJourney
    {
        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new WhenGettingSelectAddDraftApprenticeshipJourneyFixture();
            var result = await fixture.ActAsync();
            result.VerifyReturnsViewModel();
        }
    }

    public class WhenGettingSelectAddDraftApprenticeshipJourneyFixture
    {
        public CohortController Sut { get; set; }

        private readonly SelectAddDraftApprenticeshipJourneyRequest _request;
        public readonly long ProviderId = 123;
        private readonly Mock<IModelMapper> _modelMapperMock;
        public  SelectAddDraftApprenticeshipJourneyViewModel ViewModel { get; set; }

        public WhenGettingSelectAddDraftApprenticeshipJourneyFixture()
        {
            _request = new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = ProviderId };

            var fixture = new Fixture();
            ViewModel = fixture.Create<SelectAddDraftApprenticeshipJourneyViewModel>();
            ViewModel.ProviderId = ProviderId;
            ViewModel.HasExistingCohort = true;
            ViewModel.HasCreateCohortPermission = true;

            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock
                .Setup(x => x.Map<SelectAddDraftApprenticeshipJourneyViewModel>(_request))
                .ReturnsAsync(ViewModel);

            Sut = new CohortController(Mock.Of<IMediator>(),
                _modelMapperMock.Object, 
                Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
                Mock.Of<IAuthorizationService>(),
                Mock.Of<IEncodingService>(),
                 Mock.Of<IOuterApiService>());
        }

        public async Task<IActionResult> ActAsync() => await Sut.SelectAddDraftApprenticeshipJourney(_request);
    }
}