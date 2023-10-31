using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    [TestFixture]
    public class SelectAddDraftApprenticeshipJourneyViewModelTests
    {
        [TestCase(null, false)]
        [TestCase(AddDraftApprenticeshipJourneyOptions.ExistingCohort, true)]
        [TestCase(AddDraftApprenticeshipJourneyOptions.NewCohort, true)]
        public void Validate_Selection_ShouldBeValidated(AddDraftApprenticeshipJourneyOptions? selection, bool expectedValid)
        {
            var model = new SelectAddDraftApprenticeshipJourneyViewModel { Selection = selection };
            AssertValidationResult(request => request.Selection, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<SelectAddDraftApprenticeshipJourneyViewModel, T>> property, SelectAddDraftApprenticeshipJourneyViewModel instance, bool expectedValid)
        {
            var validator = new SelectAddDraftApprenticeshipJourneyViewModelValidator();
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
}