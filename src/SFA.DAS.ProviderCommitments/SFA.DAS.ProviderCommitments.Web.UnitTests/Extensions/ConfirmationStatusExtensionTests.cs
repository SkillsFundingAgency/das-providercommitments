using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions
{
    [TestFixture]
    public class ConfirmationStatusExtensionTests
    {
        [TestCase(ConfirmationStatus.Confirmed, "Confirmed")]
        [TestCase(ConfirmationStatus.Overdue, "Overdue")]
        [TestCase(ConfirmationStatus.Unconfirmed, "Unconfirmed")]
        [TestCase(null, "N/A")]
        public void ToDisplayString_Maps_Correctly(ConfirmationStatus? status, string expectedResult)
        {
            //Act
            var actualResult = status.ToDisplayString();

            //Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }
}
