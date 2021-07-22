using AutoFixture;
using NUnit.Framework;
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
        private List<DataLock> _dataLocksWithOnlyPriceMismatch;
        private List<PriceEpisode> _priceEpisodes;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _dataLocksWithCourseMismatch = new List<DataLock>();
            _dataLocksWithOnlyPriceMismatch = new List<DataLock>();
            _priceEpisodes = new List<PriceEpisode>();
            List<TrainingProgramme> TrainingProgrammes = new List<TrainingProgramme>();


            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithOnlyPriceMismatch)
              .Create();

            _priceEpisodes.Add(new PriceEpisode {ApprenticeshipId =123, FromDate = DateTime.Now.Date, ToDate = null, Cost = 1000.0M });
            _priceEpisodesResponse = _fixture.Build<GetPriceEpisodesResponse>()
                 .With(x => x.PriceEpisodes, _priceEpisodes)
                .Create();

            TrainingProgrammes.Add(new TrainingProgramme { CourseCode = "111", ProgrammeType = ProgrammeType.Standard, Name = "Training 111" });
            _allTrainingProgrammeResponse = _fixture.Build<GetAllTrainingProgrammesResponse>()
                .With(x => x.TrainingProgrammes, TrainingProgrammes)
                .Create();

            _apprenticeshipResponse = _fixture.Build<GetApprenticeshipResponse>()
             .With(p => p.CourseCode, "111")
             .With(p => p.CourseName, "Training 111")             
             .With(p => p.EndDate, DateTime.Now.Date.AddDays(100))
             .Create();
        }

        [Test]
        public void TestMapPriceDataLock()
        {
            //Act
            _dataLocksWithOnlyPriceMismatch.Add(
               new DataLock { IsResolved = false, DataLockStatus = Status.Fail, ErrorCode = DataLockErrorCode.Dlock07, IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), ApprenticeshipId = 123, IlrTotalCost = 1500.00M });
            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithOnlyPriceMismatch)
              .Create();

            var result = _priceEpisodesResponse.PriceEpisodes.MapPriceDataLock(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch);

            //Assert
            var test = result.FirstOrDefault().ApprenticeshipId;
            Assert.AreEqual(DateTime.Now.Date, result.FirstOrDefault().FromDate);
            Assert.AreEqual(null, result.FirstOrDefault().ToDate);
            Assert.AreEqual(1000.0M, result.FirstOrDefault().Cost);
            Assert.AreEqual(DateTime.Now.Date.AddDays(7), result.FirstOrDefault().IlrEffectiveFromDate);
            Assert.AreEqual(null, result.FirstOrDefault().IlrEffectiveToDate);
            Assert.AreEqual(1500.00M, result.FirstOrDefault().IlrTotalCost);
        }


        [Test]
        public void TestMapPriceDataLock_with_no_price_episodes() //TODO : Is this possible There is a Datalock -- but no price history
        {
            //Act
            _dataLocksWithOnlyPriceMismatch.Add(
               new DataLock { IsResolved = false, DataLockStatus = Status.Fail, ErrorCode = DataLockErrorCode.Dlock07, IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), ApprenticeshipId = 123, IlrTotalCost = 1500.00M });
            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithOnlyPriceMismatch)
              .Create();

            _priceEpisodesResponse.PriceEpisodes = new List<PriceEpisode>();

            var result = _priceEpisodesResponse.PriceEpisodes.MapPriceDataLock(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch);

            //Assert
            var test = result;           
        }


        [Test]
        public void TestMapPrice_DataLocks()
        {
            //Act
            _dataLocksWithOnlyPriceMismatch.Add(
               new DataLock { IsResolved = false, DataLockStatus = Status.Fail, ErrorCode = DataLockErrorCode.Dlock07, IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), ApprenticeshipId = 123, IlrTotalCost = 1500.00M });
            _dataLocksWithOnlyPriceMismatch.Add(
               new DataLock { IsResolved = false, DataLockStatus = Status.Fail, ErrorCode = DataLockErrorCode.Dlock07, IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), ApprenticeshipId = 123, IlrTotalCost = 1300.00M });
            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithOnlyPriceMismatch)
              .Create();

            _priceEpisodes = new List<PriceEpisode>();
            _priceEpisodes.Add(new PriceEpisode { ApprenticeshipId = 123, FromDate = DateTime.Now.Date, ToDate = null, Cost = 1000.0M });
            _priceEpisodes.Add(new PriceEpisode { ApprenticeshipId = 123, FromDate = DateTime.Now.Date, ToDate = null, Cost = 1200.0M });
            _priceEpisodesResponse = _fixture.Build<GetPriceEpisodesResponse>()
                 .With(x => x.PriceEpisodes, _priceEpisodes)
                .Create();


            var result = _priceEpisodesResponse.PriceEpisodes.MapPriceDataLock(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch);

            //Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void Test_MapDataLockSummary()
        {            
            //Arrange
            _dataLocksWithCourseMismatch.Add(
               new DataLock { IsResolved = false, DataLockStatus = Status.Fail, ErrorCode = DataLockErrorCode.Dlock04, IlrTrainingCourseCode = "111" });

            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithOnlyPriceMismatch)
              .Create();

            //Act
            var result = _dataLockSummariesResponse.MapDataLockSummary(_allTrainingProgrammeResponse);

            //Assert
            var test = result;
        }


        [Test]
        public void Test_MapDataLockSummary_exception()
        {
            //Arrange
            _dataLocksWithCourseMismatch.Add(
               new DataLock { IsResolved = false, DataLockStatus = Status.Fail, ErrorCode = DataLockErrorCode.Dlock04});

            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithOnlyPriceMismatch)
              .Create();

            var expectedMessage = $"Datalock {_dataLockSummariesResponse.DataLocksWithCourseMismatch.FirstOrDefault().Id} IlrTrainingCourseCode {_dataLockSummariesResponse.DataLocksWithCourseMismatch.FirstOrDefault().IlrTrainingCourseCode} not found; possible expiry";

            //Act
            //var result = _dataLockSummariesResponse.MapDataLockSummary(_allTrainingProgrammeResponse);
            var exception = Assert.Throws<InvalidOperationException>(() => _dataLockSummariesResponse.MapDataLockSummary(_allTrainingProgrammeResponse));

            //Assert
            Assert.AreEqual(expectedMessage, exception.Message);
        }



        [Test]
        public void TestMap_Course_DataLock()
        {
            //Arrange
            _apprenticeshipResponse.HasHadDataLockSuccess = false;

            _dataLocksWithCourseMismatch.Add(
                new DataLock { IsResolved = false, DataLockStatus = Status.Fail, ErrorCode = DataLockErrorCode.Dlock04, IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), IlrTrainingCourseCode = "111"  });

            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithOnlyPriceMismatch)
              .Create();

            //Act
            var dataLockSummary = _dataLockSummariesResponse.MapDataLockSummary(_allTrainingProgrammeResponse);
            var result = _apprenticeshipResponse.MapCourseDataLock(dataLockSummary.DataLockWithCourseMismatch, _priceEpisodes);

            //Assert
            var test = result.FirstOrDefault();
            Assert.AreEqual(DateTime.Now.Date, result.FirstOrDefault().FromDate);
            Assert.AreEqual(null, result.FirstOrDefault().ToDate);
            Assert.AreEqual("Training 111", result.FirstOrDefault().TrainingName);
            Assert.AreEqual(DateTime.Now.Date.AddDays(7), result.FirstOrDefault().IlrEffectiveFromDate);
            Assert.AreEqual(null, result.FirstOrDefault().IlrEffectiveToDate);
            Assert.AreEqual("Training 111", result.FirstOrDefault().IlrTrainingName);
        }


        [Test]
        public void TestMap_Course_DataLocks()
        {

            //Arrange
            _apprenticeshipResponse.HasHadDataLockSuccess = false;

            _dataLocksWithCourseMismatch.Add(
               new DataLock { IsResolved = false, DataLockStatus = Status.Fail, ErrorCode = DataLockErrorCode.Dlock04, IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), ApprenticeshipId = 123, IlrTrainingCourseCode = "111" });
            _dataLocksWithCourseMismatch.Add(
               new DataLock { IsResolved = false, DataLockStatus = Status.Fail, ErrorCode = DataLockErrorCode.Dlock04, IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), ApprenticeshipId = 123, IlrTrainingCourseCode = "111" });
            _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
              .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
              .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithOnlyPriceMismatch)
              .Create();

            _priceEpisodes = new List<PriceEpisode>();
            _priceEpisodes.Add(new PriceEpisode { ApprenticeshipId = 123, FromDate = DateTime.Now.Date, ToDate = null, Cost = 1000.0M });
            _priceEpisodes.Add(new PriceEpisode { ApprenticeshipId = 123, FromDate = DateTime.Now.Date, ToDate = null, Cost = 1200.0M });
            _priceEpisodesResponse = _fixture.Build<GetPriceEpisodesResponse>()
                 .With(x => x.PriceEpisodes, _priceEpisodes)
                .Create();


            //Act
            var dataLockSummary = _dataLockSummariesResponse.MapDataLockSummary(_allTrainingProgrammeResponse);
            var result = _apprenticeshipResponse.MapCourseDataLock(dataLockSummary.DataLockWithCourseMismatch, _priceEpisodes);

            //Assert
            Assert.AreEqual(2, result.Count());
        }

    }
}
