using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort;
public class SelectMultipleLearnerRecordsViewModelValidator : AbstractValidator<SelectMultipleLearnerRecordsViewModel>
{
    public SelectMultipleLearnerRecordsViewModelValidator()
    {
        RuleFor(x => x.SelectedLearnersIds)
            .Must((model, selectedLearnerIds) =>
                 selectedLearnerIds.Count <= model.MaxSelectableLearners)
            .WithMessage(model => $"You can select up to {model.MaxSelectableLearners} learners. Remove learners to keep adding.");

    }
}