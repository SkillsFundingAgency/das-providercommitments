using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static SFA.DAS.CommitmentsV2.Api.Types.Responses.GetPriceEpisodesResponse;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions
{
    [TestFixture]
    public class DataLockMapExtensionsTests
    {
        private Fixture _fixture;
        private GetDataLockSummariesResponse _dataLockSummariesResponse;
        private GetPriceEpisodesResponse _priceEpisodesResponse;
        private GetApprenticeshipResponse _apprenticeshipResponse;
        private GetAllTrainingProgrammesResponse _allTrainingProgrammeResponse;        
        private List<DataLock> _dataLocksWithCourseMismatch;
        private List<DataLock> _dataLocksWithPriceMismatch;
        private List<PriceEpisode> _priceEpisodes;

        [SetUp]
        public void Init()
        {
            _fixture = new Fixture();
            _priceEpisodes = new List<PriceEpisode>
            {
                new() { ApprenticeshipId = 123, FromDate = DateTime.Now.Date, ToDate = null, Cost = 1000.0M }
            };
            _priceEpisodesResponse = _fixture.Build<GetPriceEpisodesResponse>()
                 .With(x => x.PriceEpisodes, _priceEpisodes)
                .Create();

            List<TrainingProgramme> TrainingProgrammes = new List<TrainingProgramme>
            {
                new() { Name = "Software engineer", CourseCode = "548", ProgrammeType = ProgrammeType.Standard }
            };
            _allTrainingProgrammeResponse = _fixture.Build<GetAllTrainingProgrammesResponse>()
                .With(x => x.TrainingProgrammes, TrainingProgrammes)
                .Create();

            _apprenticeshipResponse = _fixture.Build<GetApprenticeshipResponse>()
             .With(p => p.CourseCode, "548")
             .With(p => p.CourseName, "DevOps engineer")             
             .With(p => p.EndDate, DateTime.Now.Date.AddDays(360))
             .With(p => p.HasHadDataLockSuccess, false)
             .Create();
        }

        [Test]
        public void PriceDataLock_IsMapped()
        {
            //Act
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
            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()              
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithPriceMismatch)
              .Create();

            var result = _priceEpisodesResponse.PriceEpisodes.MapPriceDataLock(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch);

            //Assert
            var priceDataLock = result.FirstOrDefault();
            Assert.That(priceDataLock.CurrentStartDate, Is.EqualTo(_priceEpisodes.FirstOrDefault().FromDate));
            Assert.That(priceDataLock.CurrentEndDate, Is.EqualTo(null));
            Assert.That(priceDataLock.CurrentCost, Is.EqualTo(_priceEpisodes.FirstOrDefault().Cost));
            Assert.That(priceDataLock.IlrEffectiveFromDate, Is.EqualTo(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch.FirstOrDefault().IlrEffectiveFromDate));
            Assert.That(priceDataLock.IlrEffectiveToDate, Is.EqualTo(null));
            Assert.That(priceDataLock.IlrTotalCost, Is.EqualTo(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch.FirstOrDefault().IlrTotalCost));
        }

        [Test]
        public void Multiple_PriceDataLocks_AreMapped()
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
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithPriceMismatch)
              .Create();

            _priceEpisodes = new List<PriceEpisode>
            {
                new() { ApprenticeshipId = 123, FromDate = DateTime.Now.Date, ToDate = null, Cost = 1000.0M },
                new() { ApprenticeshipId = 123, FromDate = DateTime.Now.Date, ToDate = null, Cost = 1200.0M }
            };
            _priceEpisodesResponse = _fixture.Build<GetPriceEpisodesResponse>()
                 .With(x => x.PriceEpisodes, _priceEpisodes)
                .Create();

            //Act
            var result = _priceEpisodesResponse.PriceEpisodes.MapPriceDataLock(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch);

            //Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void DataLockSummary_IsMapped()
        {
            //Arrange
            _dataLocksWithCourseMismatch = new List<DataLock>
            {
                new()
                { 
                    IsResolved = false, 
                    DataLockStatus = Status.Fail, 
                    ErrorCode = DataLockErrorCode.Dlock04, 
                    IlrTrainingCourseCode = "548" 
                }
            };

            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)              
              .Create();

            //Act
            var result = _dataLockSummariesResponse.MapDataLockSummary(_allTrainingProgrammeResponse);

            //Assert
            Assert.That(result.DataLockWithCourseMismatch.Count, Is.EqualTo(1));
        }

        [Test]
        public void DataLockSummary_Throws_Exception_If_DataLock_CourseCode_Not_Exists()
        {
            //Arrange
            _dataLocksWithCourseMismatch = new List<DataLock>
            {
                new() { IsResolved = false, DataLockStatus = Status.Fail, ErrorCode = DataLockErrorCode.Dlock04 }
            };

            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)              
              .Create();

            var expectedMessage = $"Datalock {_dataLockSummariesResponse.DataLocksWithCourseMismatch.FirstOrDefault().Id} IlrTrainingCourseCode {_dataLockSummariesResponse.DataLocksWithCourseMismatch.FirstOrDefault().IlrTrainingCourseCode} not found; possible expiry";

            //Act            
            var exception = Assert.Throws<InvalidOperationException>(() => _dataLockSummariesResponse.MapDataLockSummary(_allTrainingProgrammeResponse));

            //Assert
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }
        
        [Test]
        public void CourseDataLock_IsMapped()
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
                    IlrTrainingCourseCode = "548" 
                }
            };

            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithPriceMismatch)
              .Create();

            //Act
            var dataLockSummary = _dataLockSummariesResponse.MapDataLockSummary(_allTrainingProgrammeResponse);
            var result = _apprenticeshipResponse.MapCourseDataLock(dataLockSummary.DataLockWithCourseMismatch, _priceEpisodes);

            //Assert
            var courseDataLock = result.FirstOrDefault();
            Assert.That(courseDataLock.CurrentStartDate, Is.EqualTo(_priceEpisodes.FirstOrDefault().FromDate));
            Assert.That(courseDataLock.CurrentEndDate, Is.EqualTo(null));
            Assert.That(courseDataLock.CurrentTrainingName, Is.EqualTo(_apprenticeshipResponse.CourseName));
            Assert.That(courseDataLock.IlrEffectiveFromDate, Is.EqualTo(dataLockSummary.DataLockWithCourseMismatch.FirstOrDefault().IlrEffectiveFromDate));
            Assert.That(courseDataLock.IlrEffectiveToDate, Is.EqualTo(null));
            Assert.That(courseDataLock.IlrTrainingName, Is.EqualTo(_allTrainingProgrammeResponse.TrainingProgrammes.FirstOrDefault().Name));
        }

        [Test]
        public void Multiple_CourseDataLocks_AreMapped()
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
                },
                new()
                { 
                    IsResolved = false, 
                    DataLockStatus = Status.Fail, 
                    ErrorCode = DataLockErrorCode.Dlock05, 
                    IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                    ApprenticeshipId = 123, 
                    IlrTrainingCourseCode = "548" 
                }
            };
            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)              
              .Create();           

            //Act
            var dataLockSummary = _dataLockSummariesResponse.MapDataLockSummary(_allTrainingProgrammeResponse);
            var result = _apprenticeshipResponse.MapCourseDataLock(dataLockSummary.DataLockWithCourseMismatch, _priceEpisodes);

            //Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void PriceDataLocks_With_No_Matching_Price_Episodes_AreMapped_To_First_Price_Episode()
        {
            //Arrange
            var ilrEffectiveFromDate = DateTime.UtcNow;

            _dataLocksWithPriceMismatch = new List<DataLock>
            {
                new()
                {
                    IsResolved = false,
                    DataLockStatus = Status.Fail,
                    ErrorCode = DataLockErrorCode.Dlock07,
                    IlrEffectiveFromDate = ilrEffectiveFromDate,
                    ApprenticeshipId = 123,
                    IlrTotalCost = 1500.00M
                }
            };
            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
                .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithPriceMismatch)
                .Create();

            _priceEpisodes = new List<PriceEpisode>
            {
                new() { ApprenticeshipId = 123, FromDate = ilrEffectiveFromDate.AddDays(1), ToDate = null, Cost = 1000.0M }
            };
            _priceEpisodesResponse = _fixture.Build<GetPriceEpisodesResponse>()
                .With(x => x.PriceEpisodes, _priceEpisodes)
                .Create();

            //Act
            var result = _priceEpisodesResponse.PriceEpisodes.MapPriceDataLock(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch);

            //Assert
            Assert.That(result.Count(), Is.EqualTo(1));
        }
    }
}
