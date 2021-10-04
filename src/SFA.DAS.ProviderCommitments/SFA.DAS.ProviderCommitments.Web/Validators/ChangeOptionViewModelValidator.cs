using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class ChangeOptionViewModelValidator : AbstractValidator<ChangeOptionViewModel>
    {
        public ChangeOptionViewModelValidator()
        {
            RuleFor(r => r.SelectedOption).NotNull().WithMessage("Select an option");

            When(r => !string.IsNullOrEmpty(r.SelectedOption) && !r.ReturnToChangeVersion && !r.ReturnToEdit, () =>
            {
                RuleFor(r => r.SelectedOption).NotEqual(r => r.CurrentOption).WithMessage("Select a different option or cancel");
            });
        }
    }
}
