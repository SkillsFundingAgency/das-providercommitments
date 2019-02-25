using System;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Models;

namespace SFA.DAS.ProviderCommitments.Tests.Models
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
        public void Constructor_WithInvalidYearMonthElementValues_ShouldSetIsValidToFalse(string invalidMonthYear)
        {
            var dt = new MonthYearModel(invalidMonthYear);

            Assert.IsFalse(dt.IsValid);
        }

        [TestCase("apples")]
        [TestCase("1211111")]
        public void Constructor_WithUnrecogniseableYearMonth_ShouldSetIsValidToFalse(string invalidMonthYear)
        {
            var dt = new MonthYearModel(invalidMonthYear);

            Assert.IsFalse(dt.IsValid);
        }

        [TestCase("012019")]
        [TestCase("12019")]
        public void Constructor_WithValidYearMonth_ShouldSetIsValidToTrue(string validMonthYear)
        {
            var dt = new MonthYearModel(validMonthYear);

            Assert.IsTrue(dt.IsValid);
        }

        [Test]
        public void Day_WhenSet_ShouldThrowException()
        {
            var dt = new MonthYearModel("022019");

            Assert.Throws<InvalidOperationException>(() => dt.Day = 1);
        }
    }
}
