using System.Linq;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Enums;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class GetAllChangeHistoryRequestToViewModelMapperTests
{
    private Fixture _fixture;

    private GetAllChangeHistoryRequest _request;

    private GetAllChangeHistoryResponse _getAllChangeHistoryResponse;

    private Mock<IApprovalsOuterApiClient> _mockApprovalsApiClient;
    private Mock<IEncodingService> _encodingService;
    private GetAllChangeHistoryRequestToViewModelMapper _mapper;

    [SetUp]
    public void Arrange()
    {
        _fixture = new Fixture();

        _request = _fixture.Create<GetAllChangeHistoryRequest>();

        _getAllChangeHistoryResponse = _fixture.Build<GetAllChangeHistoryResponse>()
            .With(x => x.ChangeHistory)
            .Create();      

        _mockApprovalsApiClient = new Mock<IApprovalsOuterApiClient>();
        _mockApprovalsApiClient
            .Setup(c => c.GetAllChangeHistory(It.IsAny<long>()))
            .ReturnsAsync(_getAllChangeHistoryResponse);

        _mapper = new GetAllChangeHistoryRequestToViewModelMapper(_mockApprovalsApiClient.Object);
    }

    [Test]
    public async Task Then_AvailableFromIsMapped()
    {
        var viewModel = await _mapper.Map(_request);

        viewModel.AvailableFrom.Should().Be(_getAllChangeHistoryResponse.ChangeHistory.OrderBy(t => t.Created).FirstOrDefault().Created);
    }

    [Test]
    public async Task Then_ViewModelIsMapped()
    {
        var viewModel = await _mapper.Map(_request);

        foreach (var item in viewModel.ChangeHistory)
        {
            item.Description.Should().Be(_getAllChangeHistoryResponse.ChangeHistory.First(t => t.Id == item.Id).Description);
            item.ChangeType.Should().Be((LearningChangeType)_getAllChangeHistoryResponse.ChangeHistory.First(t => t.Id == item.Id).ChangeType);
            item.AppliedDate.Should().Be(_getAllChangeHistoryResponse.ChangeHistory.First(t => t.Id == item.Id).AppliedDate);
            item.EmployerName.Should().Be(_getAllChangeHistoryResponse.ChangeHistory.First(t => t.Id == item.Id).EmployerName);
            item.LearnerName.Should().Be(_getAllChangeHistoryResponse.ChangeHistory.First(t => t.Id == item.Id).LearnerName);
        }
    }

    [Test]
    public async Task Handle_When_ChangeHistory_IsEmpty()
    {
        _mockApprovalsApiClient
           .Setup(c => c.GetAllChangeHistory(It.IsAny<long>()))
           .ReturnsAsync(new GetAllChangeHistoryResponse() );

        var viewModel = await _mapper.Map(_request);

        viewModel.ChangeHistory.Should().BeEmpty();
    }
}