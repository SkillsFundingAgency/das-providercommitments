﻿using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using static SFA.DAS.CommitmentsV2.Api.Types.Responses.GetPriceEpisodesResponse;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions;

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

        var TrainingProgrammes = new List<TrainingProgramme>
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
        using (new AssertionScope())
        {
            priceDataLock.CurrentStartDate.Should().Be(_priceEpisodes.FirstOrDefault().FromDate);
            priceDataLock.CurrentEndDate.Should().Be(null);
            priceDataLock.CurrentCost.Should().Be(_priceEpisodes.FirstOrDefault().Cost);
            priceDataLock.IlrEffectiveFromDate.Should().Be(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch.FirstOrDefault().IlrEffectiveFromDate);
            priceDataLock.IlrEffectiveToDate.Should().Be(null);
            priceDataLock.IlrTotalCost.Should().Be(_dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch.FirstOrDefault().IlrTotalCost);
        }
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
        result.Count().Should().Be(2);
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
        result.DataLockWithCourseMismatch.Count.Should().Be(1);
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
        var action = () => _dataLockSummariesResponse.MapDataLockSummary(_allTrainingProgrammeResponse);

        //Assert
        action
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage(expectedMessage);
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
        using (new AssertionScope())
        {
            courseDataLock.CurrentStartDate.Should().Be(_priceEpisodes.FirstOrDefault().FromDate);
            courseDataLock.CurrentEndDate.Should().Be(null);
            courseDataLock.CurrentTrainingName.Should().Be(_apprenticeshipResponse.CourseName);
            courseDataLock.IlrEffectiveFromDate.Should().Be(dataLockSummary.DataLockWithCourseMismatch.FirstOrDefault().IlrEffectiveFromDate);
            courseDataLock.IlrEffectiveToDate.Should().Be(null);
            courseDataLock.IlrTrainingName.Should().Be(_allTrainingProgrammeResponse.TrainingProgrammes.FirstOrDefault().Name);
        }
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
        result.Count().Should().Be(2);
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
        result.Count().Should().Be(1);
    }
}