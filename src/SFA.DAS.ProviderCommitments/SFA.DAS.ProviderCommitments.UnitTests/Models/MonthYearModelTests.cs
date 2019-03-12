using System;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Models;

namespace SFA.DAS.ProviderCommitments.UnitTests.Models
{
    [TestFixture]
    public class MonthYearModelTests
    {
        [Test]
        public void Constructor_WithYearMonth_ShouldNotThrowException()
        {
            var dt = new MonthYearModel("022019");

            Assert.Pass("Completed without exception");
        }

        [Test]
        public void IsValid_ConstructedWithValidYearMonth_ShouldBeTrue()
        {
            var dt = new MonthYearModel("022019");

            Assert.True(dt.IsValid);
        }

        [TestCase("000000")]
        [TestCase("132019")]
        [TestCase("120000")]
        public void Constructor_WithInvalidYearMonthElementValues_ShouldThrowOutOfRangeException(string invalidMonthYear)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MonthYearModel(invalidMonthYear));
        }

        [TestCase("apples")]
        [TestCase("1211111")]
        public void Constructor_WithUnrecogniseableYearMonth_ShouldThrowInvalidArgumentException(string invalidMonthYear)
        {
            Assert.Throws<ArgumentException>(() => new MonthYearModel(invalidMonthYear));
        }

        [TestCase("012019")]
        [TestCase("12019")]
       public void Constructor_WithValidYearMonth_ShouldNotThrowException(string validMonthYear)
        {
            var dt = new MonthYearModel(validMonthYear);

            Assert.Pass("Completed without exception");
        }

        [Test]
        public void Day_WhenSet_ShouldThrowException()
        {
            var dt = new MonthYearModel("022019");

            Assert.Throws<InvalidOperationException>(() => dt.Day = 1);
        }
    }
}
