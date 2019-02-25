using System;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Models;

namespace SFA.DAS.ProviderCommitments.Tests.Models
{
    [TestFixture]
    public class DateModelTests
    {
        [Test]
        public void Constructor_WithoutDate_ShouldNotThrowException()
        {
            var dt = new DateModel();

            Assert.Pass("Completed without exception");
        }

        [Test]
        public void Constructor_WithDate_ShouldNotThrowException()
        {
            var dt = new DateModel(DateTime.Now);

            Assert.Pass("Completed without exception");
        }

        [Test]
        public void IsValid_ConstructedWithValidDate_ShouldBeTrue()
        {
            var dt = new DateModel(DateTime.Now);

            Assert.True(dt.IsValid);
        }

        [Test]
        public void IsValid_ConstructedWithoutDate_ShouldBeFalse()
        {
            var dt = new DateModel();

            Assert.False(dt.IsValid);
        }

        private const int MaxYearSupportedByDateTime = 9999;

        [TestCase(2019, 1, 1, true)]
        [TestCase(2019, 12, 31, true)]
        [TestCase(2019, 02, 29, false)]
        [TestCase(2019, 06, 31, false)]
        public void IsValid_WithSpecifiedYearMonthDay_ShouldSetIsValidCorrectly(int year, int month, int day, bool expectedIsValid)
        {
            // arrange
            var dt = new DateModel();

            dt.Year = year;
            dt.Month = month;
            dt.Day = day;

            // act
            var actualIsValid = dt.IsValid;

            // assert
            Assert.AreEqual(expectedIsValid, actualIsValid);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(32)]
        public void Day_SetToInvalidValue_ShouldThrowOutOfRangeException(int day)
        {
            var dt = new DateModel();

            Assert.Throws<ArgumentOutOfRangeException>(() => dt.Day = day);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(13)]
        public void Month_SetToInvalidValue_ShouldThrowOutOfRangeException(int month)
        {
            var dt = new DateModel();

            Assert.Throws<ArgumentOutOfRangeException>(() => dt.Month = month);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Year_SetToInvalidValue_ShouldThrowOutOfRangeException(int year)
        {
            var dt = new DateModel();

            Assert.Throws<ArgumentOutOfRangeException>(() => dt.Year = year);
        }

    }
}
