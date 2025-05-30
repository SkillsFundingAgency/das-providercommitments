using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort;

[TestFixture]
public class SelectHowToAddFirstApprenticeshipJourneyViewModelValidatorTests
{
    [TestCase(null, false)]
    [TestCase(AddFirstDraftApprenticeshipJourneyOptions.Ilr, true)]
    [TestCase(AddFirstDraftApprenticeshipJourneyOptions.Manual, true)]
    public void Validate_Selection_ShouldBeValidated(AddFirstDraftApprenticeshipJourneyOptions? selection, bool expectedValid)
    {
        var model = new SelectHowToAddFirstApprenticeshipJourneyViewModel { Selection = selection };
        AssertValidationResult(request => request.Selection, model, expectedValid);
    }

    private static void AssertValidationResult<T>(Expression<Func<SelectHowToAddFirstApprenticeshipJourneyViewModel, T>> property, SelectHowToAddFirstApprenticeshipJourneyViewModel instance, bool expectedValid)
    {
        var validator = new SelectHowToAddFirstApprenticeshipJourneyViewModelValidator();
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