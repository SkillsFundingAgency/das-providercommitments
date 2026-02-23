using SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeship;

[TestFixture]
public class DraftApprenticeshipViewModelSetReferenceApimRequestMapperTests
{
    private DraftApprenticeshipViewModelSetReferenceApimRequestMapper _mapper;
    private readonly Fixture _fixture = new();
    private DraftApprenticeshipSetReferenceViewModel _viewModel;

    [SetUp]
    public void Setup()
    {
        _viewModel = _fixture.Create<DraftApprenticeshipSetReferenceViewModel>();
        _mapper = new DraftApprenticeshipViewModelSetReferenceApimRequestMapper();
    }

    [Test]
    public async Task Reference_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_viewModel);
        result.Reference.Should().Be(_viewModel.Reference);
    }

    [Test]
    public async Task Party_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_viewModel);
        result.Party.Should().Be(_viewModel.Party);
    }
}
