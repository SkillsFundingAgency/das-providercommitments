using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions
{
    public class MonthYearStringExtensionsTests
    {
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("XXXX", false)]
        [TestCase("132020", false)]
        [TestCase("022020", true)]
        [TestCase("22020", true)]
        public void ThenIsValidMonthYearCheckIsCalled(string dateMonth, bool isValid)
        {
            //Act
            var actual = dateMonth.IsValidMonthYear();

            //Assert
            Assert.AreEqual(isValid, actual);
        }
    }
}