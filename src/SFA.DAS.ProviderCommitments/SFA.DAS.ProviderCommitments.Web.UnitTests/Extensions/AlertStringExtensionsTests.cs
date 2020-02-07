using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions
{
    public class AlertStringExtensionsTests
    {
        [TestCase(Alerts.IlrDataMismatch, "ILR data mismatch")]
        [TestCase(Alerts.ChangesForReview, "Changes for review")]
        [TestCase(Alerts.ChangesPending, "Changes pending")]
        [TestCase(Alerts.ChangesRequested, "Changes requested")]
        public void ThenAlertsAreFormattedCorrectly(Alerts alert, string expected)
        {
            //Act
            var actual = alert.FormatAlert();

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}