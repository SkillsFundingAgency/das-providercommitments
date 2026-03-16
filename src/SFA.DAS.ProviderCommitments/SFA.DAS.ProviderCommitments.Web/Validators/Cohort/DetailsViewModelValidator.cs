using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class DetailsViewModelValidator : AbstractValidator<DetailsViewModel>
    {
        public DetailsViewModelValidator()
        {
            RuleFor(x => x.RplVerified)
                .Equal(true)
                .When(x => x.Selection == CohortDetailsOptions.Send || x.Selection == CohortDetailsOptions.Approve)
                .WithMessage("Check the box to confirm you have checked RPL for each apprentice");
            RuleFor(x => x.Selection).NotEmpty().WithMessage("You must choose an option");
        }
    }
}
