using System;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Extensions;

namespace SFA.DAS.ProviderCommitments.UnitTests.Extensions.DateTimeExtensionsTests
{
    [TestFixture]
    public class WhenGettingFirstOfMonth
    {
        [TestCase("2018-06-15", "2018-06-01 00:00:00")]
        [TestCase("2018-06-01 18:35:14", "2018-06-01 00:00:00")]
        public void ThenTheFirstDayOfTheMonthIsReturned(DateTime value, DateTime expectResult)
        {
            Assert.AreEqual(expectResult, value.FirstOfMonth());
        }
    }
}