using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions
{
    public class ApprenticeshipStatusStringExtensionsTests
    {
        [TestCase(ApprenticeshipStatus.Completed, "Completed")]
        [TestCase(ApprenticeshipStatus.Live, "Live")]
        [TestCase(ApprenticeshipStatus.Paused, "Paused")]
        [TestCase(ApprenticeshipStatus.Stopped, "Stopped")]
        [TestCase(ApprenticeshipStatus.WaitingToStart, "Waiting To Start")]
        public void ThenApprenticeshipStatusIsFormattedCorrectly(ApprenticeshipStatus status, string expected)
        {
            //Act
            var actual = status.FormatStatus();

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}