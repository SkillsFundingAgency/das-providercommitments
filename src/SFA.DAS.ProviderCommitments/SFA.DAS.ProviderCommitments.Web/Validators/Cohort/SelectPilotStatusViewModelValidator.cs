using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class SelectPilotStatusViewModelValidator : AbstractValidator<SelectPilotStatusViewModel>
    {
        public SelectPilotStatusViewModelValidator()
        {
            RuleFor(x => x.Selection).NotEmpty().WithMessage("You must select a pilot status");
        }
    }
}
