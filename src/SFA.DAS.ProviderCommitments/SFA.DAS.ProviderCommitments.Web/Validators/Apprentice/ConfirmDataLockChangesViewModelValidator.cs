using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class ConfirmDataLockChangesViewModelValidator : AbstractValidator<ConfirmDataLockChangesViewModel>
    {
        public ConfirmDataLockChangesViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();
            RuleFor(x => x.SubmitStatusViewModel).NotNull().WithMessage("Confirm if you want to make these changes");
        }
    }
}
