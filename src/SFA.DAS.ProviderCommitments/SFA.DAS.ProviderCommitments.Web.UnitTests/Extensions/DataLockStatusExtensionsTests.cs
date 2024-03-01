using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions
{
    [TestFixture]
    public class DataLockStatusExtensionsTests
    {
        private GetManageApprenticeshipDetailsResponse.DataLock _dataLock;

        [SetUp]
        public void Arrange()
        {
            _dataLock = new GetManageApprenticeshipDetailsResponse.DataLock();
        }

        [TestCase(DataLockErrorCode.Dlock03, true)]
        [TestCase(DataLockErrorCode.Dlock04, true)]
        [TestCase(DataLockErrorCode.Dlock05, true)]
        [TestCase(DataLockErrorCode.Dlock03 | DataLockErrorCode.Dlock04, true)]
        [TestCase(DataLockErrorCode.Dlock03 | DataLockErrorCode.Dlock07, true)]
        [TestCase(DataLockErrorCode.Dlock07, false)]
        public void HasCourseDataLock_Returns_Correct_Value(DataLockErrorCode errorCode, bool expectedResult)
        {
            _dataLock.ErrorCode = errorCode;
            Assert.That(_dataLock.HasCourseDataLock(), Is.EqualTo(expectedResult));
        }
    }
}
