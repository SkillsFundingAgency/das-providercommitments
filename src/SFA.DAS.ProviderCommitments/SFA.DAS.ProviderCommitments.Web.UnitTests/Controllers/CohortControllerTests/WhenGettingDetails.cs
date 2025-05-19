using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests;

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

    [TestCase(CommitmentsV2.Types.Party.Employer)]
    [TestCase(CommitmentsV2.Types.Party.TransferSender)]
    public async Task ThenViewModelIsReadOnlyIfCohortIsNotWithProvider(Party withParty)
    {
        _fixture.WithParty(withParty);
        await _fixture.GetDetails();
        _fixture.IsViewModelReadOnly().Should().BeTrue();
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
            _viewModel.WithParty = Party.Employer;

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
                Mock.Of<IAuthorizationService>(), 
                Mock.Of<ILogger<CohortController>>()
            );
        }

        public WhenGettingDetailsTestFixture WithParty(Party withParty)
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

            viewModel.Should().BeAssignableTo<DetailsViewModel>();
            var detailsViewModel = (DetailsViewModel)viewModel;

            detailsViewModel.Should().Be(_viewModel);

            var expectedTotalCost = _viewModel.Courses?.Sum(g => g.DraftApprenticeships.Sum(a => a.Cost ?? 0)) ?? 0;
            _viewModel.TotalCost.Should().Be(expectedTotalCost, "The total cost stored in the model is incorrect");
        }

        public bool IsViewModelReadOnly()
        {
            return _viewModel.IsReadOnly;
        }
    }
}