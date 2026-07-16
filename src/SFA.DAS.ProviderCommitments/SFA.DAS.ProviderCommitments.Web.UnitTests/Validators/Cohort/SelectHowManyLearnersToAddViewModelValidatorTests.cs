using System;
using System.Linq.Expressions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort;

[TestFixture]
public class SelectHowManyLearnersToAddViewModelValidatorTests
{
    [TestCase(null, false)]
    [TestCase(HowManyLearnersToAddOptions.Single, true)]
    [TestCase(HowManyLearnersToAddOptions.Multiple, true)]
    public void Validate_Selection_ShouldBeValidated(HowManyLearnersToAddOptions? selection, bool expectedValid)
    {
        var model = new SelectHowManyLearnersToAddViewModel { Selection = selection };
        AssertValidationResult(request => request.Selection, model, expectedValid);
    }

    private static void AssertValidationResult<T>(Expression<Func<SelectHowManyLearnersToAddViewModel, T>> property, SelectHowManyLearnersToAddViewModel instance, bool expectedValid)
    {
        var validator = new SelectHowManyLearnersToAddViewModelValidator();
        var result = validator.TestValidate(instance);

        if (expectedValid)
        {
            result.ShouldNotHaveValidationErrorFor(property);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(property);
        }
    }
}
