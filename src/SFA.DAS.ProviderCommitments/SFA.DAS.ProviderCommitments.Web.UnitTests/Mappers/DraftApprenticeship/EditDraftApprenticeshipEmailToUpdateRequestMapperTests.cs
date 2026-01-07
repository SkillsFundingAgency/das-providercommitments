using SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeship;

[TestFixture]
public class EditDraftApprenticeshipEmailToUpdateRequestMapperTests
{
    private EditDraftApprenticeshipEmailToUpdateRequestMapper _mapper;
    private readonly Fixture _fixture = new();
    private DraftApprenticeshipAddEmailViewModel _viewModel;

    [SetUp]
    public void Setup()
    {
        
        _viewModel = _fixture.Create<DraftApprenticeshipAddEmailViewModel>();
        _mapper = new EditDraftApprenticeshipEmailToUpdateRequestMapper();
    }

    [Test]
    public async Task Email_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_viewModel);
        result.Email.Should().Be(_viewModel.Email);
    }
}
