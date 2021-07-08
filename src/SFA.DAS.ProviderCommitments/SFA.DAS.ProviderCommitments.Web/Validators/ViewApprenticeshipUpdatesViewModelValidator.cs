using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class ViewApprenticeshipUpdatesViewModelValidator : AbstractValidator<ViewApprenticeshipUpdatesViewModel>
    {
        public ViewApprenticeshipUpdatesViewModelValidator()
        {
            RuleFor(r => r.UndoChanges).NotNull()
                .WithMessage("Confirm if you want to undo these changes");
        }
    }
}