using System;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using static SFA.DAS.CommitmentsV2.Api.Types.Responses.GetPriceEpisodesResponse;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class ConfirmDataLockChangesViewModelMapperTests
    {
        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
        private ConfirmDataLockChangesRequest _confirmDataLockChangesRequest;
        private ConfirmDataLockChangesViewModelMapper _mapper;
        private GetDataLockSummariesResponse _getDataLockSummariesResponse;
        private GetApprenticeshipResponse _getApprenticeshipResponse;
        private GetAllTrainingProgrammesResponse _getAllTrainingProgrammesResponse;
        private List<DataLock> _dataLocksWithCourseMismatch;
        private List<TrainingProgramme> _trainingProgrammes;
        private GetPriceEpisodesResponse _getPriceEpisodesResponse;
        private readonly List<PriceEpisode> _priceEpisodes = new();

        [SetUp]
        public void Arrange()
        {
            var autoFixture = new Fixture();

            _confirmDataLockChangesRequest = autoFixture.Create<ConfirmDataLockChangesRequest>();

            _dataLocksWithCourseMismatch = new List<DataLock>
            {
                new()
                {
                    IsResolved = false,
                    DataLockStatus = Status.Fail,
                    ErrorCode = DataLockErrorCode.Dlock04,
                    IlrTrainingCourseCode = "454-3-1",
                    IlrEffectiveFromDate = DateTime.Now.Date.AddDays(15)
                }
            };
            _getDataLockSummariesResponse = autoFixture.Build<GetDataLockSummariesResponse>().With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch).Create();

            _getApprenticeshipResponse = autoFixture.Build<GetApprenticeshipResponse>()
                .With(p => p.CourseCode, "111")
                .With(p => p.CourseName, "Training 111")
                .With(p => p.EndDate, DateTime.Now.Date.AddDays(100))
                .Create();


            _trainingProgrammes = new List<TrainingProgramme>
            {
                new() { CourseCode = "454-3-1", ProgrammeType = ProgrammeType.Standard, Name = "Training 111" }
            };
            _getAllTrainingProgrammesResponse = autoFixture.Build<GetAllTrainingProgrammesResponse>().With(x => x.TrainingProgrammes, _trainingProgrammes).Create();

            _priceEpisodes.Add(new PriceEpisode { FromDate = DateTime.Now.Date, ToDate = null, Cost = 1000.0M });
            _getPriceEpisodesResponse = autoFixture.Build<GetPriceEpisodesResponse>()
                 .With(x => x.PriceEpisodes, _priceEpisodes)
                .Create();


            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockCommitmentsApiClient.Setup(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_getDataLockSummariesResponse);
            _mockCommitmentsApiClient.Setup(m => m.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_getApprenticeshipResponse);
            _mockCommitmentsApiClient.Setup(m => m.GetAllTrainingProgrammes(It.IsAny<CancellationToken>()))
               .ReturnsAsync(_getAllTrainingProgrammesResponse);
            _mockCommitmentsApiClient.Setup(m => m.GetPriceEpisodes(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_getPriceEpisodesResponse);

            _mapper = new ConfirmDataLockChangesViewModelMapper(_mockCommitmentsApiClient.Object);
        }

        [Test]
        public async Task ApprenticeshipHashedId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            Assert.That(result.ApprenticeshipHashedId, Is.EqualTo(_confirmDataLockChangesRequest.ApprenticeshipHashedId));
        }

        [Test]
        public async Task ApprenticeshipId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            Assert.That(result.ApprenticeshipId, Is.EqualTo(_confirmDataLockChangesRequest.ApprenticeshipId));
        }

        [Test]
        public async Task FirstName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            Assert.That(result.FirstName, Is.EqualTo(_getApprenticeshipResponse.FirstName));
        }

        [Test]
        public async Task LastName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            Assert.That(result.LastName, Is.EqualTo(_getApprenticeshipResponse.LastName));
        }

        [Test]
        public async Task ULN_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            Assert.That(result.ULN, Is.EqualTo(_getApprenticeshipResponse.Uln));
        }

        [Test]
        public async Task DateOfBirth_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            Assert.That(result.DateOfBirth, Is.EqualTo(_getApprenticeshipResponse.DateOfBirth));
        }

        [Test]
        public async Task CourseName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            Assert.That(result.CourseName, Is.EqualTo(_getApprenticeshipResponse.CourseName));
        }

        [Test]
        public async Task ProviderId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            Assert.That(result.ProviderId, Is.EqualTo(_getApprenticeshipResponse.ProviderId));
        }

        [Test]
        public async Task ProviderName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            Assert.That(result.ProviderName, Is.EqualTo(_getApprenticeshipResponse.ProviderName));
        }

        [Test]
        public async Task GetDraftApprenticeshipIsCalled()
        {
            //Act
            await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetApprenticeshipDataLockSummariesStatusIsCalled()
        {
            //Act
            await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetAllTrainingProgrammesIsCalled()
        {
            //Act
            await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetAllTrainingProgrammes(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetPriceEpisodesIsCalled()
        {
            //Act
            await _mapper.Map(_confirmDataLockChangesRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetPriceEpisodes(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
