using System;
using System.Linq;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Enums;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
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
            .Setup(c => c.GetAllChangeHistory(It.IsAny<GetAllChangeHistoryApiRequest>()))
            .ReturnsAsync(_getAllChangeHistoryResponse);

        _mapper = new GetAllChangeHistoryRequestToViewModelMapper(_mockApprovalsApiClient.Object);
    }

    [Test]
    public async Task Then_AvailableFromIsMapped()
    {
        var viewModel = await _mapper.Map(_request);

        viewModel.AvailableFrom.Should().Be(_getAllChangeHistoryResponse.AvailableFrom);
    }

    [Test]
    public async Task Then_AvailableFromIsNullWhenNoHistory()
    {
        _mockApprovalsApiClient
            .Setup(c => c.GetAllChangeHistory(It.IsAny<GetAllChangeHistoryApiRequest>()))
            .ReturnsAsync(new GetAllChangeHistoryResponse { AvailableFrom = null, TotalCount = 0 });

        var viewModel = await _mapper.Map(_request);

        viewModel.AvailableFrom.Should().BeNull();
    }

    [Test]
    public async Task Then_PageNumberIsClampedToOneWhenZero()
    {
        _request.PageNumber = 0;

        await _mapper.Map(_request);

        _mockApprovalsApiClient.Verify(c => c.GetAllChangeHistory(It.Is<GetAllChangeHistoryApiRequest>(r => r.PageNumber == 1)), Times.Once);
    }

    [Test]
    public async Task Then_ValidPageNumberIsPassedThroughUnchanged()
    {
        _request.PageNumber = 3;
        _getAllChangeHistoryResponse.TotalCount = 120;

        var viewModel = await _mapper.Map(_request);

        _mockApprovalsApiClient.Verify(c => c.GetAllChangeHistory(It.Is<GetAllChangeHistoryApiRequest>(r => r.PageNumber == 3)), Times.Once);
        viewModel.PageNumber.Should().Be(3);
    }

    [Test]
    public async Task Then_PageNumberPastLastPageIsPassedThroughWithSingleApiCall()
    {
        _request.PageNumber = 5;
        _mockApprovalsApiClient
            .Setup(c => c.GetAllChangeHistory(It.IsAny<GetAllChangeHistoryApiRequest>()))
            .ReturnsAsync(new GetAllChangeHistoryResponse { TotalCount = 120 });

        var viewModel = await _mapper.Map(_request);

        _mockApprovalsApiClient.Verify(c => c.GetAllChangeHistory(It.IsAny<GetAllChangeHistoryApiRequest>()), Times.Once);
        viewModel.PageNumber.Should().Be(5);
        viewModel.IsPageOutOfRange.Should().BeTrue();
    }

    [Test]
    public async Task Then_PageNumberWithNoChangesIsNotOutOfRange()
    {
        _request.PageNumber = 5;
        _mockApprovalsApiClient
            .Setup(c => c.GetAllChangeHistory(It.IsAny<GetAllChangeHistoryApiRequest>()))
            .ReturnsAsync(new GetAllChangeHistoryResponse { TotalCount = 0 });

        var viewModel = await _mapper.Map(_request);

        viewModel.PageNumber.Should().Be(5);
        viewModel.IsPageOutOfRange.Should().BeFalse();
        viewModel.ChangeHistory.Should().BeEmpty();
    }

    [Test]
    public async Task Then_TotalCountIsMapped()
    {
        var viewModel = await _mapper.Map(_request);

        viewModel.TotalCount.Should().Be(_getAllChangeHistoryResponse.TotalCount);
    }

    [Test]
    public async Task Then_AvailableFromIsNotDerivedFromCurrentPage()
    {
        var earlier = _getAllChangeHistoryResponse.ChangeHistory.Min(t => t.Created).AddDays(-1);
        _getAllChangeHistoryResponse.ChangeHistory.Add(new GetChangeHistoryItem { Created = earlier });
        _getAllChangeHistoryResponse.AvailableFrom = new DateTime(2026, 1, 1);

        var viewModel = await _mapper.Map(_request);

        viewModel.AvailableFrom.Should().Be(new DateTime(2026, 1, 1));
    }

    [Test]
    public async Task Then_AvailableFromIsMappedWhenChangeHistoryIsEmpty()
    {
        var availableFrom = new DateTime(2026, 1, 1);
        _mockApprovalsApiClient
            .Setup(c => c.GetAllChangeHistory(It.IsAny<GetAllChangeHistoryApiRequest>()))
            .ReturnsAsync(new GetAllChangeHistoryResponse { AvailableFrom = availableFrom, TotalCount = 0 });

        var viewModel = await _mapper.Map(_request);

        viewModel.AvailableFrom.Should().Be(availableFrom);
        viewModel.ChangeHistory.Should().BeEmpty();
        viewModel.TotalCount.Should().Be(0);
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
           .Setup(c => c.GetAllChangeHistory(It.IsAny<GetAllChangeHistoryApiRequest>()))
           .ReturnsAsync(new GetAllChangeHistoryResponse() );

        var viewModel = await _mapper.Map(_request);

        viewModel.ChangeHistory.Should().BeEmpty();
    }
}