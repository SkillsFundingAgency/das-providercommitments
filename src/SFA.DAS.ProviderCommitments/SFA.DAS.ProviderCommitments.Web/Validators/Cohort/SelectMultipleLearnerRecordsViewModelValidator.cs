using FluentValidation;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort;
public class SelectMultipleLearnerRecordsViewModelValidator : AbstractValidator<SelectMultipleLearnerRecordsViewModel>
{
    public SelectMultipleLearnerRecordsViewModelValidator()
    {
        When(x => x.LevyStatus == ApprenticeshipEmployerType.Levy, () =>
        {
            RuleFor(x => x.SelectedLearners)
            .Must((model, selectedLearners) =>
                 selectedLearners.Count <= model.MaxSelectableLearners)
            .WithMessage(model => $"You can select up to {model.MaxSelectableLearners} learners. Remove learners to keep adding.");
        });
        When(x => x.LevyStatus == ApprenticeshipEmployerType.NonLevy, () =>
        {
            RuleFor(x => x.SelectedLearners)
            .Must((model, selectedLearners) =>
                 selectedLearners.Count <= model.MaxSelectableLearners)
            .WithMessage(model => "You’ve reached your reservation limit. Remove learners from the list or go to Manage funding to delete reservations you’re not using.");
        });
    }
}