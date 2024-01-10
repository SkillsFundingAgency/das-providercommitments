using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using static SFA.DAS.CommitmentsV2.Api.Types.Responses.GetPriceEpisodesResponse;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class UpdateDataLockRequestViewModelMapperTests
    {
        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
        private UpdateDateLockRequest _updateDataLockRequest;
        private UpdateDataLockRequestViewModelMapper _mapper;
        private GetDataLockSummariesResponse _dataLockSummariesResponse;
        private GetApprenticeshipResponse _apprenticeshipResponse;
        private GetAllTrainingProgrammesResponse _allTrainingProgrammesResponse;
        private List<DataLock> _dataLocksWithCourseMismatch;
        private List<TrainingProgramme> _trainingProgrammes;
        private GetPriceEpisodesResponse _priceEpisodesResponse;
        private List<PriceEpisode> _priceEpisodes;
        private List<DataLock> _dataLocksWithPriceMismatch;
        private Fixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _updateDataLockRequest = _fixture.Create<UpdateDateLockRequest>();
            _dataLocksWithPriceMismatch = new List<DataLock>
            {
                new()
                { 
                    IsResolved = false, 
                    DataLockStatus = Status.Fail, 
                    ErrorCode = DataLockErrorCode.Dlock07, 
                    IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                    ApprenticeshipId = 123, 
                    IlrTotalCost = 1500.00M 
                }
            };

            _dataLocksWithCourseMismatch = new List<DataLock>
            {
                new()
                {
                    IsResolved = false,
                    DataLockStatus = Status.Fail,
                    ErrorCode = DataLockErrorCode.Dlock04,
                    IlrTrainingCourseCode = "548",
                    IlrEffectiveFromDate = DateTime.Now.Date.AddDays(15)
                }
            };           

            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithPriceMismatch)
              .Create();

            _apprenticeshipResponse = _fixture.Build<GetApprenticeshipResponse>()
                .With(p => p.CourseCode, "548")
                .With(p => p.CourseName, "DevOps engineer")
                .With(p => p.HasHadDataLockSuccess, false)
                .With(p => p.EndDate, DateTime.Now.Date.AddDays(100))
                .Create();

            _trainingProgrammes = new List<TrainingProgramme>
            {
                new() { CourseCode = "548", ProgrammeType = ProgrammeType.Standard, Name = "DevOps engineer" }
            };
            _allTrainingProgrammesResponse = _fixture.Build<GetAllTrainingProgrammesResponse>().With(x => x.TrainingProgrammes, _trainingProgrammes).Create();

            _priceEpisodes = new List<PriceEpisode>
            {
                new() { FromDate = DateTime.Now.Date, ToDate = null, Cost = 1000.0M }
            };
            _priceEpisodesResponse = _fixture.Build<GetPriceEpisodesResponse>()
                 .With(x => x.PriceEpisodes, _priceEpisodes)
                 .Create();

            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockCommitmentsApiClient.Setup(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_dataLockSummariesResponse);
            _mockCommitmentsApiClient.Setup(m => m.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_apprenticeshipResponse);
            _mockCommitmentsApiClient.Setup(m => m.GetAllTrainingProgrammes(It.IsAny<CancellationToken>()))
               .ReturnsAsync(_allTrainingProgrammesResponse);
            _mockCommitmentsApiClient.Setup(m => m.GetPriceEpisodes(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_priceEpisodesResponse);

            _mapper = new UpdateDataLockRequestViewModelMapper(_mockCommitmentsApiClient.Object);
        }

        [Test]
        public async Task ApprenticeshipHashedId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.That(result.ApprenticeshipHashedId, Is.EqualTo(_updateDataLockRequest.ApprenticeshipHashedId));
        }

        [Test]
        public async Task ApprenticeshipId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.That(result.ApprenticeshipId, Is.EqualTo(_updateDataLockRequest.ApprenticeshipId));
        }

        [Test]
        public async Task FirstName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.That(result.FirstName, Is.EqualTo(_apprenticeshipResponse.FirstName));
        }

        [Test]
        public async Task LastName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.That(result.LastName, Is.EqualTo(_apprenticeshipResponse.LastName));
        }

        [Test]
        public async Task ULN_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.That(result.ULN, Is.EqualTo(_apprenticeshipResponse.Uln));
        }

        [Test]
        public async Task DateOfBirth_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.That(result.DateOfBirth, Is.EqualTo(_apprenticeshipResponse.DateOfBirth));
        }

        [Test]
        public async Task CourseName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.That(result.CourseName, Is.EqualTo(_apprenticeshipResponse.CourseName));
        }

        [Test]
        public async Task ProviderId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.That(result.ProviderId, Is.EqualTo(_apprenticeshipResponse.ProviderId));
        }

        [Test]
        public async Task ProviderName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.That(result.ProviderName, Is.EqualTo(_apprenticeshipResponse.ProviderName));
        }

        [Test]
        public async Task GetDraftApprenticeshipIsCalled()
        {
            //Act
            await _mapper.Map(_updateDataLockRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetApprenticeshipDatalockSummariesStatusIsCalled()
        {
            //Act
            await _mapper.Map(_updateDataLockRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetAllTrainingProgrammesIsCalled()
        {
            //Act
            await _mapper.Map(_updateDataLockRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetAllTrainingProgrammes(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetPriceEpisodesIsCalled()
        {
            //Act
            await _mapper.Map(_updateDataLockRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetPriceEpisodes(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }       

        [Test]
        public async Task Price_Datalock_Mismatch_IsMapped()
        {
            //Arrange            
            _dataLocksWithPriceMismatch = new List<DataLock>
            {
                new()
                { 
                    IsResolved = false, 
                    DataLockStatus = Status.Fail, 
                    ErrorCode = DataLockErrorCode.Dlock07, 
                    IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                    ApprenticeshipId = 123, 
                    IlrTotalCost = 1500.00M 
                },
            };
            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithPriceMismatch)
              .Create();

            _mockCommitmentsApiClient.Setup(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(_dataLockSummariesResponse);
           
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert            
            Assert.That(result.PriceDataLocks.Count(), Is.EqualTo(1));
            Assert.That(_priceEpisodes.FirstOrDefault().Cost, Is.EqualTo(result.PriceDataLocks.FirstOrDefault().CurrentCost));
            Assert.That(_priceEpisodes.FirstOrDefault().ToDate, Is.EqualTo(result.PriceDataLocks.FirstOrDefault().CurrentEndDate));
            Assert.That(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch.FirstOrDefault().IlrTotalCost, Is.EqualTo(result.PriceDataLocks.FirstOrDefault().IlrTotalCost));
            Assert.That(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch.FirstOrDefault().IlrEffectiveFromDate, Is.EqualTo(result.PriceDataLocks.FirstOrDefault().IlrEffectiveFromDate));
        }

        [Test]
        public async Task Course_Datalock_Mismatch_IsMapped()
        {
            //Arrange
            _dataLocksWithCourseMismatch = new List<DataLock>
            {
                new()
                { 
                    IsResolved = false, 
                    DataLockStatus = Status.Fail, 
                    ErrorCode = DataLockErrorCode.Dlock04,
                    IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                    ApprenticeshipId = 123, 
                    IlrTrainingCourseCode = "548" 
                }
            };
            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)              
              .Create();

            _mockCommitmentsApiClient.Setup(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(_dataLockSummariesResponse);        

            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert            
            Assert.That(result.CourseDataLocks.Count(), Is.EqualTo(1));
            Assert.That(_apprenticeshipResponse.CourseName, Is.EqualTo(result.CourseDataLocks.FirstOrDefault().CurrentTrainingName));
            Assert.That(_priceEpisodes.FirstOrDefault().ToDate, Is.EqualTo(result.CourseDataLocks.FirstOrDefault().CurrentEndDate));
            Assert.That(_allTrainingProgrammesResponse.TrainingProgrammes.FirstOrDefault().Name, Is.EqualTo(result.CourseDataLocks.FirstOrDefault().IlrTrainingName));
            Assert.That(_dataLockSummariesResponse.DataLocksWithCourseMismatch.FirstOrDefault().IlrEffectiveFromDate, Is.EqualTo(result.CourseDataLocks.FirstOrDefault().IlrEffectiveFromDate));
        }
        
        [Test]
        public async Task Multiple_Price_Datalock_Mismatch_IsMapped()
        {
            //Arrange                        
            _dataLocksWithPriceMismatch = new List<DataLock>
            {
                new()
                { 
                    IsResolved = false, 
                    DataLockStatus = Status.Fail, 
                    ErrorCode = DataLockErrorCode.Dlock07, 
                    IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                    ApprenticeshipId = 123, 
                    IlrTotalCost = 1500.00M 
                },
                new()
                { 
                    IsResolved = false, 
                    DataLockStatus = Status.Fail, 
                    ErrorCode = DataLockErrorCode.Dlock07, 
                    IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                    ApprenticeshipId = 123, 
                    IlrTotalCost = 1300.00M 
                }
            };
            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithPriceMismatch)
              .Create();

            _mockCommitmentsApiClient.Setup(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(_dataLockSummariesResponse);

            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert            
            Assert.That(result.PriceDataLocks.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Course_And_price_Datalock_Mismatch_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert            
            Assert.That(result.CourseDataLocks.Count(), Is.EqualTo(1));
            Assert.That(result.PriceDataLocks.Count(), Is.EqualTo(1));
        }
    }
}
