﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Extensions;

namespace SFA.DAS.ProviderCommitments.UnitTests.Extensions.ITrainingCourseExtensionsTests;

[TestFixture]
public class WhenDeterminingFundingCap
{
    private TrainingProgramme _course;

    [SetUp]
    public void Arrange()
    {
        _course = new TrainingProgramme
        {
            Name = "Test",
            EffectiveFrom = new DateTime(2018, 03, 01),
            EffectiveTo = new DateTime(2019, 03, 31),
            FundingPeriods = new List<TrainingProgrammeFundingPeriod>
            {
                new()
                {
                    EffectiveFrom = new DateTime(2018,03,01),
                    EffectiveTo = new DateTime(2018,07,31),
                    FundingCap = 5000
                },
                new()
                {
                    EffectiveFrom = new DateTime(2018,08,01),
                    EffectiveTo = null,
                    FundingCap = 2000
                }
            }
        };
    }

    [TestCase("2018-05-15", 5000, Description = "Within first funding band")]
    [TestCase("2018-09-15", 2000, Description = "Within second funding band")]
    [TestCase("2018-01-01", 0, Description = "Before course start")]
    [TestCase("2019-06-01", 0, Description = "After course end")]
    public void ThenTheApplicableFundingPeriodIsUsed(DateTime effectiveDate, int expectCap)
    {
        //Act
        var result = _course.FundingCapOn(effectiveDate);

        //Assert
        result.Should().Be(expectCap);
    }

    [TestCase("2020-01-01", 0, Description = "Before funding band")]
    [TestCase("2020-02-01", 1, Description = "Before funding band but in same month")]
    [TestCase("2020-02-20", 1, Description = "Within open-ended funding band first day in same month")]
    [TestCase("2020-02-21", 1, Description = "Within open-ended funding band in same month")]
    [TestCase("2020-03-01", 1, Description = "Within open-ended funding band")]
    public void AndOnlyFundingPeriodHasEffectiveFromNotFirstOfMonthAndEffectiveToOpenEndedThenTheApplicableFundingPeriodIsUsed(DateTime effectiveDate, int expectCap)
    {
        var courseAndFundingBandStart = new DateTime(2020, 2, 20);
        var courseAndFundingBandEnd = (DateTime?)null;

        _course.EffectiveFrom = courseAndFundingBandStart;
        _course.EffectiveTo = courseAndFundingBandEnd;

        _course.FundingPeriods = new List<TrainingProgrammeFundingPeriod>
        {
            new()
            {
                EffectiveFrom = courseAndFundingBandStart,
                EffectiveTo = courseAndFundingBandEnd,
                FundingCap = 1
            }
        };

        //Act
        var result = _course.FundingCapOn(effectiveDate);

        //Assert
        result.Should().Be(expectCap);
    }

    [TestCase("2018-07-01", 1, Description = "Within first open-start funding band")]
    [TestCase("2018-07-31", 1, Description = "At end of first open-start funding band")]
    [TestCase("2018-08-01", 2, Description = "Start of second funding band")]
    [TestCase("2020-01-01", 2, Description = "Within second open-ended funding band")]
    public void AndFirstFundingPeriodHasNullEffectiveFromThenTheApplicableFundingPeriodIsUsed(DateTime effectiveDate, int expectCap)
    {
        _course.EffectiveFrom = null;
        _course.EffectiveTo = null;

        _course.FundingPeriods = new List<TrainingProgrammeFundingPeriod>
        {
            new()
            {
                EffectiveFrom = null,
                EffectiveTo = new DateTime(2018,07,31),
                FundingCap = 1
            },
            new()
            {
                EffectiveFrom = new DateTime(2018,08,01),
                EffectiveTo = null,
                FundingCap = 2
            }
        };

        //Act
        var result = _course.FundingCapOn(effectiveDate);

        //Assert
        result.Should().Be(expectCap);
    }

    [TestCase("2018-07-31", 0, Description = "Before first funding band")]
    [TestCase("2018-08-01", 1, Description = "Before first funding band but withing same month as first funding band")]
    [TestCase("2018-08-15", 1, Description = "Within (only-day of) first funding band")]
    [TestCase("2018-08-16", 2, Description = "Within (only-day of) second funding band")]
    [TestCase("2018-08-17", 3, Description = "At start of third funding band")]
    [TestCase("2018-08-26", 0, Description = "Beyond end of last funding band")]
    public void AndMultipleBandsInMonthThenTheApplicableFundingPeriodIsUsed(DateTime effectiveDate, int expectCap)
    {
        var courseAndFundingBandStart = new DateTime(2018, 08, 15);
        var courseAndFundingBandEnd = new DateTime(2018, 08, 25);

        _course.EffectiveFrom = courseAndFundingBandStart;
        _course.EffectiveTo = courseAndFundingBandEnd;

        _course.FundingPeriods = new List<TrainingProgrammeFundingPeriod>
        {
            new()
            {
                EffectiveFrom = courseAndFundingBandStart,
                EffectiveTo = new DateTime(2018,08,15),
                FundingCap = 1
            },
            new()
            {
                EffectiveFrom = new DateTime(2018,08,16),
                EffectiveTo = new DateTime(2018,08,16),
                FundingCap = 2
            },
            new()
            {
                EffectiveFrom = new DateTime(2018,08,17),
                EffectiveTo = courseAndFundingBandEnd,
                FundingCap = 3
            }
        };

        //Act
        var result = _course.FundingCapOn(effectiveDate);

        //Assert
        result.Should().Be(expectCap);
    }

    [TestCase("2018-08-01", 1, Description = "Within first open-start funding band")]
    [TestCase("2018-08-15", 1, Description = "At end of first open-start funding band")]
    [TestCase("2018-08-16", 2, Description = "At start of second funding band (in same month as first)")]
    [TestCase("2018-08-21", 3, Description = "Start of third funding band")]
    [TestCase("2020-01-01", 3, Description = "Within third open-ended funding band")]
    public void AndFirstFundingPeriodHasNullEffectiveFromAndMultipleBandsInMonthThenTheApplicableFundingPeriodIsUsed(DateTime effectiveDate, int expectCap)
    {
        _course.EffectiveFrom = null;
        _course.EffectiveTo = null;

        _course.FundingPeriods = new List<TrainingProgrammeFundingPeriod>
        {
            new()
            {
                EffectiveFrom = null,
                EffectiveTo = new DateTime(2018,08,15),
                FundingCap = 1
            },
            new()
            {
                EffectiveFrom = new DateTime(2018,08,16),
                EffectiveTo = new DateTime(2018,08,20),
                FundingCap = 2
            },
            new()
            {
                EffectiveFrom = new DateTime(2018,08,21),
                EffectiveTo = null,
                FundingCap = 3
            }
        };

        //Act
        var result = _course.FundingCapOn(effectiveDate);

        //Assert
        result.Should().Be(expectCap);
    }

    [Test]
    public void IfThereAreNoFundingPeriodsThenCapShouldBeZero()
    {
        //Arrange
        _course.FundingPeriods = new List<TrainingProgrammeFundingPeriod>();

        //Act
        var result = _course.FundingCapOn(new DateTime(2018, 05, 15));

        //Assert
        result.Should().Be(0);
    }

    [Test]
    public void FundingPeriodsAreEffectiveUntilTheEndOfTheDay()
    {
        //Act
        var result = _course.FundingCapOn(new DateTime(2018, 7, 31, 23, 59, 59));

        //Assert
        result.Should().Be(5000);
    }
}