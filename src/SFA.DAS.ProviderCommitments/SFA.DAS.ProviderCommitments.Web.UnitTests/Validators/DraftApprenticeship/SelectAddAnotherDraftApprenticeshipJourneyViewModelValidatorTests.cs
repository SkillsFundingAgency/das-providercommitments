using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Validators.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.DraftApprenticeship;

[TestFixture]
public class SelectAddAnotherDraftApprenticeshipJourneyViewModelValidatorTests
{
    [TestCase(null, false)]
    [TestCase(AddAnotherDraftApprenticeshipJourneyOptions.Ilr, true)]
    [TestCase(AddAnotherDraftApprenticeshipJourneyOptions.Manual, true)]
    public void Validate_Selection_ShouldBeValidated(AddAnotherDraftApprenticeshipJourneyOptions? selection, bool expectedValid)
    {
        var model = new SelectAddAnotherDraftApprenticeshipJourneyViewModel { Selection = selection };
        AssertValidationResult(request => request.Selection, model, expectedValid);
    }

    private static void AssertValidationResult<T>(Expression<Func<SelectAddAnotherDraftApprenticeshipJourneyViewModel, T>> property, SelectAddAnotherDraftApprenticeshipJourneyViewModel instance, bool expectedValid)
    {
        var validator = new SelectAddAnotherDraftApprenticeshipJourneyViewModelValidator();
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