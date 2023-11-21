using AutoFixture;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenPostingRecognisePriorLearningSummaryRequest
    {
        public DraftApprenticeshipController Sut { get; set; }
        private Mock<IModelMapper> _modelMapperMock;
        private Mock<IAuthorizationService> _providerFeatureToggle;
        private PriorLearningSummaryViewModel _viewModel;

        [SetUp]
        public void Arrange()
        {
            var _autoFixture = new Fixture();
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = _autoFixture.Create<PriorLearningSummaryViewModel>();

            _providerFeatureToggle = new Mock<IAuthorizationService>();
            _providerFeatureToggle.Setup(x => x.IsAuthorized(It.IsAny<string>())).Returns(false);

            Sut = new DraftApprenticeshipController(
                Mock.Of<IMediator>(),
                Mock.Of<ICommitmentsApiClient>(),
                _modelMapperMock.Object,
                Mock.Of<IEncodingService>(),
                _providerFeatureToggle.Object,
                Mock.Of<IOuterApiService>(),
                Mock.Of<IAuthenticationService>(),
                Mock.Of<ILogger<DraftApprenticeshipController>>());
        }

        [Test]
        public void When_posting_from_Recognise_Prior_Learning_Summary()
        {
            var action = Sut.RecognisePriorLearningSummary(_viewModel);
            action.VerifyReturnsRedirectToActionResult().WithActionName("Details");
        }
    }
}