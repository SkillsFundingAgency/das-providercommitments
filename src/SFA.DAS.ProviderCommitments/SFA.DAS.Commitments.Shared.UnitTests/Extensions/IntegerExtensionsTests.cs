using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Extensions;

namespace SFA.DAS.Commitments.Shared.UnitTests.Extensions
{
    [TestFixture]
    public class IntegerExtensionsTests
    {
        [TestCase(1, "£1")]
        [TestCase(0, "£0")]
        [TestCase(123456, "£123,456")]
        public void ToGdsCostFormatReturnsFormattedResultCorrectly(int value, string expectedResult)
        {
            Assert.AreEqual(expectedResult, value.ToGdsCostFormat());
        }

    }
}
