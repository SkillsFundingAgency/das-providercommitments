﻿using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Types;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class DataLockRequestRestartViewModelMapperTests
    {
        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;        
        private DataLockRequestRestartRequest _dataLockRequestRestartRequest;        
        private DataLockRequestRestartRequestToViewModelMapper _mapper;
        private GetDataLockSummariesResponse _getDataLockSummariesResponse;
        private GetApprenticeshipResponse _getApprenticeshipResponse;
        private GetAllTrainingProgrammesResponse _getAllTrainingProgrammesResponse;
        private List<DataLock> DataLocksWithCourseMismatch;
        private List<TrainingProgramme> TrainingProgrammes;

        [SetUp]
        public void Arrange()
        {
            var _autoFixture = new Fixture();

            _dataLockRequestRestartRequest = _autoFixture.Create<DataLockRequestRestartRequest>();

            DataLocksWithCourseMismatch = new List<DataLock>
            {
                new DataLock
                {
                    IsResolved = false,
                    DataLockStatus = Status.Fail,
                    ErrorCode = DataLockErrorCode.Dlock04,
                    IlrTrainingCourseCode = "454-3-1",
                    IlrEffectiveFromDate = DateTime.Now.Date.AddDays(15)
                }
            };
            _getDataLockSummariesResponse = _autoFixture.Build<GetDataLockSummariesResponse>().With(x => x.DataLocksWithCourseMismatch, DataLocksWithCourseMismatch).Create();
            
            _getApprenticeshipResponse = _autoFixture.Build<GetApprenticeshipResponse>()
                .With(p => p.CourseCode, "111")
                .With(p => p.CourseName, "Training 111")
                .With(p => p.EndDate, DateTime.Now.Date.AddDays(100))
                .Create();


            TrainingProgrammes = new List<TrainingProgramme>
            {
                new TrainingProgramme { CourseCode = "454-3-1", ProgrammeType = ProgrammeType.Standard, Name = "Training 111" }
            };
            _getAllTrainingProgrammesResponse = _autoFixture.Build<GetAllTrainingProgrammesResponse>().With(x => x.TrainingProgrammes, TrainingProgrammes).Create();

            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockCommitmentsApiClient.Setup(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_getDataLockSummariesResponse);
            _mockCommitmentsApiClient.Setup(m => m.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_getApprenticeshipResponse);
            _mockCommitmentsApiClient.Setup(m => m.GetAllTrainingProgrammes(It.IsAny<CancellationToken>()))
               .ReturnsAsync(_getAllTrainingProgrammesResponse);

            _mapper = new DataLockRequestRestartRequestToViewModelMapper(_mockCommitmentsApiClient.Object);
        }   

        [Test]
        public async Task ApprenticeshipHashedId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            Assert.AreEqual(_dataLockRequestRestartRequest.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ApprenticeshipId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            Assert.AreEqual(_dataLockRequestRestartRequest.ApprenticeshipId, result.ApprenticeshipId);
        }

        [Test]
        public async Task FirstName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            Assert.AreEqual(_getApprenticeshipResponse.FirstName, result.FirstName);
        }

        [Test]
        public async Task LastName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            Assert.AreEqual(_getApprenticeshipResponse.LastName, result.LastName);
        }

        [Test]
        public async Task ULN_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            Assert.AreEqual(_getApprenticeshipResponse.Uln, result.ULN);
        }

        [Test]
        public async Task CourseName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            Assert.AreEqual(_getApprenticeshipResponse.CourseName, result.CourseName);
        }

        [Test]
        public async Task ProviderId_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            Assert.AreEqual(_getApprenticeshipResponse.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ProviderName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            Assert.AreEqual(_getApprenticeshipResponse.ProviderName, result.ProviderName);
        }

        [Test]
        public async Task NewCourseCode_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert           
            Assert.AreEqual(_getAllTrainingProgrammesResponse.TrainingProgrammes.FirstOrDefault().CourseCode, result.NewCourseCode);
        }

        [Test]
        public async Task NewCourseName_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert            
            Assert.AreEqual(_getAllTrainingProgrammesResponse.TrainingProgrammes.FirstOrDefault().Name, result.NewCourseName);
        } 

        [Test]
        public async Task IlrEffectiveFromDate_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert            
            Assert.AreEqual(_getDataLockSummariesResponse.DataLocksWithCourseMismatch.FirstOrDefault().IlrEffectiveFromDate, result.IlrEffectiveFromDate);
        }

        [Test]
        public async Task IlrEffectiveToDate_IsMapped()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            Assert.AreEqual(_getDataLockSummariesResponse.DataLocksWithCourseMismatch.FirstOrDefault().IlrPriceEffectiveToDate, result.IlrEffectiveToDate);
        }

        [Test]
        public async Task GetDraftApprenticeshipIsCalled()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetApprenticeshipDatalockSummariesStatusIsCalled()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetAllTrainingProgrammesIsCalled()
        {
            //Act
            var result = await _mapper.Map(_dataLockRequestRestartRequest);

            //Assert
            _mockCommitmentsApiClient.Verify(m => m.GetAllTrainingProgrammes(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
