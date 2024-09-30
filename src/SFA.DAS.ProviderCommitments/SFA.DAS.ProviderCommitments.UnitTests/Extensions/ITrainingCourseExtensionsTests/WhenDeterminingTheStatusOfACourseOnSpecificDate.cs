using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Extensions;

namespace SFA.DAS.ProviderCommitments.UnitTests.Extensions.ITrainingCourseExtensionsTests;

[TestFixture]
public class WhenDeterminingTheStatusOfACourseOnSpecificDate
{
    [TestCase("2016-01-01", "2016-12-01", "2016-06-01", true, Description = "Within date range")]
    [TestCase("2016-01-01", "2016-12-01", "2016-01-01", true, Description = "Within date range (on start day)")]
    [TestCase("2016-01-01", "2016-12-01", "2016-12-01", true, Description = "Within date range (on last day)")]
    [TestCase("2016-01-15", "2016-12-15", "2016-01-01", true, Description = "Within date range - ignoring start day")]
    //[TestCase("2016-01-15", "2016-12-15", "2016-12-30", TrainingCourseStatus.Active, Description = "Within date range - ignoring end day")]
    [TestCase("2016-01-15", "2016-12-15", "2016-12-30", false, Description = "Past date range (but in same month as courseEnd")]
    [TestCase(null, "2016-12-01", "2016-06-01", true, Description = "Within date range with no defined course start date")]
    [TestCase("2016-01-01", null, "2016-06-01", true, Description = "Withing date range, with no defined course end date")]
    [TestCase(null, null, "2016-06-01", true, Description = "Within date range, with no defined course effective dates")]
    [TestCase("2016-01-01", "2016-12-01", "2015-06-01", false, Description = "Outside (before) date range")]
    [TestCase("2016-01-01", "2016-12-01", "2015-12-31", false, Description = "Outside (immediately before) date range")]
    [TestCase("2016-01-01", "2016-12-01", "2017-06-01", false, Description = "Outside (after) date range")]
    [TestCase("2016-01-01", "2016-12-01", "2017-01-01", false, Description = "Outside (immediately after) date range")]
    [TestCase(null, "2016-12-01", "2017-06-01", false, Description = "Outside (after) date range with no defined course start date")]
    public void ThenTheCourseEffectiveDatesAreUsedToDetermineTheStatus(DateTime? courseStart, DateTime? courseEnd, DateTime effectiveDate, bool expectStatus)
    {
        //Arrange
        var course = new TrainingProgramme {EffectiveFrom = courseStart, EffectiveTo = courseEnd};

        //Act
        var result = course.IsActiveOn(effectiveDate);

        //Assert
        result.Should().Be(expectStatus);
    }
}