using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions
{
    public class AlertStringExtensionsTests
    {
        [TestCase(Alerts.IlrDataMismatch, "ILR Data Mismatch")]
        [TestCase(Alerts.ChangesForReview, "Changes For Review")]
        [TestCase(Alerts.ChangesPending, "Changes Pending")]
        [TestCase(Alerts.ChangesRequested, "Changes Requested")]
        public void ThenAlertsAreFormattedCorrectly(Alerts alert, string expected)
        {
            //Act
            var actual = alert.FormatAlert();

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}