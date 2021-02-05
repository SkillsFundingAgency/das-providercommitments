using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Extensions;

namespace SFA.DAS.ProviderCommitments.UnitTests.Extensions.ITrainingCourseExtensionsTests
{
    [TestFixture]
    public class WhenDeterminingWhetherACourseIsActive
    {
        [TestCase("2016-01-01", "2016-12-01", "2016-06-01", true, Description = "Within date range")]
        [TestCase("2016-01-15", "2016-12-15", "2016-01-01", true, Description = "Within date range - ignoring start day")]
        [TestCase("2016-01-15", "2016-12-15", "2016-12-30", false, Description = "After date range (but in same month as courseEnd")]
        [TestCase(null, "2016-12-01", "2016-06-01", true, Description = "Within date range with no defined course start date")]
        [TestCase("2016-01-01", null, "2016-06-01", true, Description = "Within date range, with no defined course end date")]
        [TestCase(null, null, "2016-06-01", true, Description = "Within date range, with no defined course effective dates")]
        [TestCase("2016-01-01", "2016-12-01", "2015-06-01", false, Description = "Outside (before) date range")]
        [TestCase("2016-01-01", "2016-12-01", "2017-06-01", false, Description = "Outside (after) date range")]
        public void ThenIfWithinCourseEffectiveRangeThenIsActive(DateTime? trainingProgrammeStart, DateTime? courseEnd, DateTime effectiveDate, bool expectIsActive)
        {
            //Arrange
            var course = new TrainingProgramme {EffectiveFrom = trainingProgrammeStart, EffectiveTo = courseEnd};

            //Act
            var result = course.IsActiveOn(effectiveDate);

            //Assert
            Assert.AreEqual(expectIsActive, result);
        }
    }
}