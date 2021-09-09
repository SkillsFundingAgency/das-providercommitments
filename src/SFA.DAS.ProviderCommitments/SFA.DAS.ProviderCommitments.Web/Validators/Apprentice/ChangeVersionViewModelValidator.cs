using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class ChangeVersionViewModelValidator : AbstractValidator<ChangeVersionViewModel>
    {
        public ChangeVersionViewModelValidator()
        {
            RuleFor(x => x.SelectedVersion).NotEmpty().WithMessage("Select a version");
        }
    }
}
