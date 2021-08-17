using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class DetailsViewModelValidator : AbstractValidator<DetailsViewModel>
    {
        public DetailsViewModelValidator()
        {
            RuleFor(x => x.Selection).NotEmpty().WithMessage("You must choose an option");
        }
    }
}
