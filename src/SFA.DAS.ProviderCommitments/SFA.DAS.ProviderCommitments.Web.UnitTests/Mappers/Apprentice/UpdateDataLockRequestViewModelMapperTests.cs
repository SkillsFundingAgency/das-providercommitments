using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Types;
using System.Linq;
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
                new DataLock 
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
                new DataLock
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
                new TrainingProgramme { CourseCode = "548", ProgrammeType = ProgrammeType.Standard, Name = "DevOps engineer" }
            };
            _allTrainingProgrammesResponse = _fixture.Build<GetAllTrainingProgrammesResponse>().With(x => x.TrainingProgrammes, _trainingProgrammes).Create();

            _priceEpisodes = new List<PriceEpisode>
            {
                new PriceEpisode { FromDate = DateTime.Now.Date, ToDate = null, Cost = 1000.0M }
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
            Assert.AreEqual(_updateDataLockRequest.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ApprenticeshipId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.AreEqual(_updateDataLockRequest.ApprenticeshipId, result.ApprenticeshipId);
        }

        [Test]
        public async Task FirstName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.AreEqual(_apprenticeshipResponse.FirstName, result.FirstName);
        }

        [Test]
        public async Task LastName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.AreEqual(_apprenticeshipResponse.LastName, result.LastName);
        }

        [Test]
        public async Task ULN_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.AreEqual(_apprenticeshipResponse.Uln, result.ULN);
        }

        [Test]
        public async Task DateOfBirth_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.AreEqual(_apprenticeshipResponse.DateOfBirth, result.DateOfBirth);
        }

        [Test]
        public async Task CourseName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.AreEqual(_apprenticeshipResponse.CourseName, result.CourseName);
        }

        [Test]
        public async Task ProviderId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.AreEqual(_apprenticeshipResponse.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ProviderName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            Assert.AreEqual(_apprenticeshipResponse.ProviderName, result.ProviderName);
        }

        [Test]
        public async Task GetDraftApprenticeshipIsCalled()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetApprenticeshipDatalockSummariesStatusIsCalled()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetAllTrainingProgrammesIsCalled()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetAllTrainingProgrammes(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetPriceEpisodesIsCalled()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetPriceEpisodes(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }       

        [Test]
        public async Task Price_Datalock_Mismatch_IsMapped()
        {
            //Arrange            
            _dataLocksWithPriceMismatch = new List<DataLock>
            {
                new DataLock { 
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
            Assert.AreEqual(1, result.PriceDataLocks.Count());
            Assert.AreEqual(result.PriceDataLocks.FirstOrDefault().CurrentCost, _priceEpisodes.FirstOrDefault().Cost);
            Assert.AreEqual(result.PriceDataLocks.FirstOrDefault().CurrentEndDate, _priceEpisodes.FirstOrDefault().ToDate);
            Assert.AreEqual(result.PriceDataLocks.FirstOrDefault().IlrTotalCost, _dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch.FirstOrDefault().IlrTotalCost);
            Assert.AreEqual(result.PriceDataLocks.FirstOrDefault().IlrEffectiveFromDate, _dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch.FirstOrDefault().IlrEffectiveFromDate);
        }

        [Test]
        public async Task Course_Datalock_Mismatch_IsMapped()
        {
            //Arrange
            _dataLocksWithCourseMismatch = new List<DataLock>
            {
                new DataLock { 
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
            Assert.AreEqual(1, result.CourseDataLocks.Count());
            Assert.AreEqual(result.CourseDataLocks.FirstOrDefault().CurrentTrainingName, _apprenticeshipResponse.CourseName);
            Assert.AreEqual(result.CourseDataLocks.FirstOrDefault().CurrentEndDate, _priceEpisodes.FirstOrDefault().ToDate);
            Assert.AreEqual(result.CourseDataLocks.FirstOrDefault().IlrTrainingName, _allTrainingProgrammesResponse.TrainingProgrammes.FirstOrDefault().Name);
            Assert.AreEqual(result.CourseDataLocks.FirstOrDefault().IlrEffectiveFromDate, _dataLockSummariesResponse.DataLocksWithCourseMismatch.FirstOrDefault().IlrEffectiveFromDate);
        }
        
        [Test]
        public async Task Multiple_Price_Datalock_Mismatch_IsMapped()
        {
            //Arrange                        
            _dataLocksWithPriceMismatch = new List<DataLock>
            {
                new DataLock { 
                    IsResolved = false, 
                    DataLockStatus = Status.Fail, 
                    ErrorCode = DataLockErrorCode.Dlock07, 
                    IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                    ApprenticeshipId = 123, 
                    IlrTotalCost = 1500.00M 
                },
                new DataLock { 
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
            Assert.AreEqual(2, result.PriceDataLocks.Count());
        }

        [Test]
        public async Task Course_And_price_Datalock_Mismatch_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_updateDataLockRequest);

            //Assert            
            Assert.AreEqual(1, result.CourseDataLocks.Count());
            Assert.AreEqual(1, result.PriceDataLocks.Count());
        }
    }
}
