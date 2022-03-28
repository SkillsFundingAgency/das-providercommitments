using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class SelectAddDraftApprenticeshipJourneyViewModelValidator : AbstractValidator<SelectAddDraftApprenticeshipJourneyViewModel>
    {
        public SelectAddDraftApprenticeshipJourneyViewModelValidator()
        {
            RuleFor(x => x.Selection).NotNull().WithMessage("You need to select how you want to add apprentice details");
        }
    }
}