using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Queries.GetEmployer;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourse;

namespace SFA.DAS.ProviderCommitments.Tests.Queries.GetTrainingCourse
{
    [TestFixture]
    public class GetTrainingCourseValidatorTests
    {
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("   ", false)]
        [TestCase("1234", true)]
        public void Valid_WithSpecifiedInput_ReturnsExpectedResults(string courseCode, bool expectedIsValid)
        {
            // arrange
            var request = new GetTrainingCourseRequest
            {
                CourseCode = courseCode
            };

            var validator = new GetTrainingCourseValidator();

            var validationResult = validator.Validate(request);

            // act
            var actualIsValid = validationResult.IsValid;

            // assert
            Assert.AreEqual(expectedIsValid, actualIsValid);
        }
    }
}
