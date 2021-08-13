using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class ViewSelectOptionsViewModelValidator : AbstractValidator<ViewSelectOptionsViewModel>
    {
        public ViewSelectOptionsViewModelValidator ()
        {
            RuleFor(x => x.SelectedOption).NotNull().WithMessage("You must choose an option");
        }
    }
}