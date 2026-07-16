using System.Linq;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort;

public class SelectMultipleLearnerRecordsViewModelValidatorTests
{    
    [TestCase(99, 100, true)]
    [TestCase(100, 100, true)]
    [TestCase(101, 100, false)]
    public void ThenSelectedLearnersCountIsValidated_ForLeavyEmployer(
        int selectedLearnersCount,
        int maxSelectableLearners,
        bool expectedValid)
    {
        var request = new SelectMultipleLearnerRecordsViewModel
        {
            MaxSelectableLearners = maxSelectableLearners,
            SelectedLearners = Enumerable
                .Range(1, selectedLearnersCount)
                .Select(_ => new LearnerSummary())
                .ToList(),
            LevyStatus = ApprenticeshipEmployerType.Levy
        };

        var validator = new SelectMultipleLearnerRecordsViewModelValidator();
        var result = validator.Validate(request);

        result.Errors
            .All(x => x.PropertyName != nameof(request.SelectedLearners))
            .Should()
            .Be(expectedValid);
    }

    [Test]
    public void ThenValidationMessageContainsMaxSelectableLearners_ForLeavyEmployer()
    {
        var request = new SelectMultipleLearnerRecordsViewModel
        {
            MaxSelectableLearners = 2,
            SelectedLearners =
            [
                new LearnerSummary(),
                new LearnerSummary(),
                new LearnerSummary()
            ],
            LevyStatus = ApprenticeshipEmployerType.Levy
        };

        var validator = new SelectMultipleLearnerRecordsViewModelValidator();
        var result = validator.Validate(request);

        result.Errors
            .Single(x => x.PropertyName == nameof(request.SelectedLearners))
            .ErrorMessage
            .Should()
            .Be("You can select up to 2 learners. Remove learners to keep adding.");
    }

    [TestCase(0, 1, true)]
    [TestCase(1, 1, true)]
    [TestCase(2, 1, false)]
    public void ThenSelectedLearnersCountIsValidated_ForNonLevyEmployer(
       int selectedLearnersCount,
       int maxSelectableLearners,
       bool expectedValid)
    {
        var request = new SelectMultipleLearnerRecordsViewModel
        {
            MaxSelectableLearners = maxSelectableLearners,
            SelectedLearners = Enumerable
                .Range(1, selectedLearnersCount)
                .Select(_ => new LearnerSummary())
                .ToList(),
            LevyStatus = ApprenticeshipEmployerType.NonLevy
        };

        var validator = new SelectMultipleLearnerRecordsViewModelValidator();
        var result = validator.Validate(request);

        result.Errors
            .All(x => x.PropertyName != nameof(request.SelectedLearners))
            .Should()
            .Be(expectedValid);
    }

    [Test]
    public void ThenValidationMessageContainsMaxSelectableLearners_ForNonLeavyEmployer()
    {
        var request = new SelectMultipleLearnerRecordsViewModel
        {
            MaxSelectableLearners = 2,
            SelectedLearners =
            [
                new LearnerSummary(),
                new LearnerSummary(),
                new LearnerSummary()
            ],
            LevyStatus = ApprenticeshipEmployerType.NonLevy
        };

        var validator = new SelectMultipleLearnerRecordsViewModelValidator();
        var result = validator.Validate(request);

        result.Errors
            .Single(x => x.PropertyName == nameof(request.SelectedLearners))
            .ErrorMessage
            .Should()
            .Be("You’ve reached your reservation limit. Remove learners from the list or go to Manage funding to delete reservations you’re not using.");
    }
}