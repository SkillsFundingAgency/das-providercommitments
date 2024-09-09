using System.Linq;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort;

public class SelectCourseViewModelValidatorTests
{
    [TestCase("", false)]
    [TestCase(null, false)]
    [TestCase("test", true)]
    public void ThenCourseCodeIsValidated(string courseCode, bool expectedValid)
    {
        var request = new SelectCourseViewModel { CourseCode = courseCode };

        var validator = new SelectCourseViewModelValidator();
        var result = validator.Validate(request);

        result.Errors.All(x => x.PropertyName != nameof(request.CourseCode)).Should().Be(expectedValid);
    }
}