using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class DeleteConfirmationViewModelMapperTests
    {
        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
        private Mock<IEncodingService> _mockencodingService;
        private DeleteConfirmationRequest  _deleteConfirmationRequest;        
        private GetDraftApprenticeshipResponse _getDraftApprenticeshipResponse;
        private DeleteConfirmationViewModelMapper _mapper;
        
        [SetUp]
        public void Arrange()
        {
            var _autoFixture = new Fixture();

            _deleteConfirmationRequest = _autoFixture.Create<DeleteConfirmationRequest>();            

            _getDraftApprenticeshipResponse = _autoFixture.Create<GetDraftApprenticeshipResponse>();

            _mockencodingService = new Mock<IEncodingService>();
            _mockencodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.CohortReference)).Returns(It.IsAny<long>);
            _mockencodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.ApprenticeshipId)).Returns(It.IsAny<long>);

            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockCommitmentsApiClient.Setup(m => m.GetDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_getDraftApprenticeshipResponse);

            _mapper = new DeleteConfirmationViewModelMapper(_mockCommitmentsApiClient.Object, _mockencodingService.Object, Mock.Of<ILogger<DeleteConfirmationViewModelMapper>>());
        }


        [Test]
        public async Task ProviderId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_deleteConfirmationRequest);

            //Assert
            Assert.AreEqual(_deleteConfirmationRequest.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task CommitmentHashedId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_deleteConfirmationRequest);

            //Assert
            Assert.AreEqual(_deleteConfirmationRequest.CohortReference, result.CohortReference);
        }


        [Test]
        public async Task ApprenticeshipHashedId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_deleteConfirmationRequest);

            //Assert
            Assert.AreEqual(_deleteConfirmationRequest.DraftApprenticeshipHashedId, result.DraftApprenticeshipHashedId);
        }

        [Test]
        public async Task ApprenticeshipName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_deleteConfirmationRequest);

            //Assert
            Assert.AreEqual($"{_getDraftApprenticeshipResponse.FirstName} {_getDraftApprenticeshipResponse.LastName}", result.ApprenticeshipName);
        }

        [Test]
        public async Task DateOfBirth_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_deleteConfirmationRequest);

            //Assert
            Assert.AreEqual(_getDraftApprenticeshipResponse.DateOfBirth, result.DateOfBirth);
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
}
