using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System.Linq;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingDetails
    {
        private WhenGettingDetailsTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenGettingDetailsTestFixture();
        }

        [Test]
        public async Task ThenViewModelShouldBeMappedFromRequest()
        {
            await _fixture.GetDetails();
            _fixture.VerifyViewModelIsMappedFromRequest();
        }

        [TestCase(Party.Employer)]
        [TestCase(Party.TransferSender)]
        public async Task ThenViewModelIsReadOnlyIfCohortIsNotWithProvider(Infrastructure.OuterApi.Responses.Party withParty)
        {
            _fixture.WithParty(withParty);
            await _fixture.GetDetails();
            Assert.That(_fixture.IsViewModelReadOnly(), Is.True);
        }

        public class WhenGettingDetailsTestFixture
        {
            private readonly DetailsRequest _request;
            private readonly DetailsViewModel _viewModel;
            private IActionResult _result;
            private readonly CohortController _cohortController;

            public WhenGettingDetailsTestFixture()
            {
                var autoFixture = new Fixture();

                _request = autoFixture.Create<DetailsRequest>();
                _viewModel = autoFixture.Create<DetailsViewModel>();
                _viewModel.HasNoDeclaredStandards = false;
                _viewModel.WithParty = Infrastructure.OuterApi.Responses.Party.Employer;

                var modelMapper = new Mock<IModelMapper>();
                modelMapper.Setup(x => x.Map<DetailsViewModel>(It.Is<DetailsRequest>(r => r == _request)))
                    .ReturnsAsync(_viewModel);

                var linkGeneratorResult = autoFixture.Create<string>();
                var linkGenerator = new Mock<ILinkGenerator>();
                linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                    .Returns(linkGeneratorResult);

                _cohortController = new CohortController(Mock.Of<IMediator>(),
                     modelMapper.Object,
                     linkGenerator.Object,
                    Mock.Of<ICommitmentsApiClient>(), 
                     Mock.Of<IEncodingService>(),
                     Mock.Of<IOuterApiService>(),
                     Mock.Of<IAuthorizationService>()
                     );
            }

            public WhenGettingDetailsTestFixture WithParty(Infrastructure.OuterApi.Responses.Party withParty)
            {
                _viewModel.WithParty = withParty;
                return this;
            }

            public async Task GetDetails()
            {
                _result = await _cohortController.Details(_request);
            }

            public void VerifyViewModelIsMappedFromRequest()
            {
                var viewResult = (ViewResult)_result;
                var viewModel = viewResult.Model;

                Assert.That(viewModel, Is.InstanceOf<DetailsViewModel>());
                var detailsViewModel = (DetailsViewModel)viewModel;

                Assert.That(detailsViewModel, Is.EqualTo(_viewModel));

                var expectedTotalCost = _viewModel.Courses?.Sum(g => g.DraftApprenticeships.Sum(a => a.Cost ?? 0)) ?? 0;
                Assert.That(_viewModel.TotalCost, Is.EqualTo(expectedTotalCost), "The total cost stored in the model is incorrect");
            }

            public bool IsViewModelReadOnly()
            {
                return _viewModel.IsReadOnly;
            }
        }
    }
}
