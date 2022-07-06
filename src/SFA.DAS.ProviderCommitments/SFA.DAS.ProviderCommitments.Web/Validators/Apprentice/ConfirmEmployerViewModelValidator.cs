using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class ConfirmEmployerViewModelValidator : AbstractValidator<ConfirmEmployerViewModel>
    {
        public ConfirmEmployerViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.Confirm).NotNull().WithMessage("Please select an option");
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();
        }
    }
}
