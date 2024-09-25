using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class DeleteConfirmationViewModelMapperTests
{
    private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
    private Mock<IEncodingService> _mockEncodingService;
    private DeleteConfirmationRequest  _deleteConfirmationRequest;        
    private GetDraftApprenticeshipResponse _getDraftApprenticeshipResponse;
    private DeleteConfirmationViewModelMapper _mapper;
        
    [SetUp]
    public void Arrange()
    {
        var autoFixture = new Fixture();

        _deleteConfirmationRequest = autoFixture.Create<DeleteConfirmationRequest>();            

        _getDraftApprenticeshipResponse = autoFixture.Create<GetDraftApprenticeshipResponse>();

        _mockEncodingService = new Mock<IEncodingService>();
        _mockEncodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.CohortReference)).Returns(It.IsAny<long>);
        _mockEncodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.ApprenticeshipId)).Returns(It.IsAny<long>);

        _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
        _mockCommitmentsApiClient.Setup(m => m.GetDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_getDraftApprenticeshipResponse);

        _mapper = new DeleteConfirmationViewModelMapper(_mockCommitmentsApiClient.Object, _mockEncodingService.Object, Mock.Of<ILogger<DeleteConfirmationViewModelMapper>>());
    }


    [Test]
    public async Task ProviderId_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_deleteConfirmationRequest);

        //Assert
        result.ProviderId.Should().Be(_deleteConfirmationRequest.ProviderId);
    }

    [Test]
    public async Task CommitmentHashedId_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_deleteConfirmationRequest);

        //Assert
        result.CohortReference.Should().Be(_deleteConfirmationRequest.CohortReference);
    }


    [Test]
    public async Task ApprenticeshipHashedId_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_deleteConfirmationRequest);

        //Assert
        result.DraftApprenticeshipHashedId.Should().Be(_deleteConfirmationRequest.DraftApprenticeshipHashedId);
    }

    [Test]
    public async Task ApprenticeshipName_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_deleteConfirmationRequest);

        //Assert
        result.ApprenticeshipName.Should().Be($"{_getDraftApprenticeshipResponse.FirstName} {_getDraftApprenticeshipResponse.LastName}");
    }

    [Test]
    public async Task DateOfBirth_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_deleteConfirmationRequest);

        //Assert
        result.DateOfBirth.Should().Be(_getDraftApprenticeshipResponse.DateOfBirth);
    }

    [Test]
    public async Task GetDraftApprenticeshipIsCalled()
    {
        //Act
        var result = await _mapper.Map(_deleteConfirmationRequest);

        //Assert
        _mockCommitmentsApiClient.Verify(m => m.GetDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
    }
}