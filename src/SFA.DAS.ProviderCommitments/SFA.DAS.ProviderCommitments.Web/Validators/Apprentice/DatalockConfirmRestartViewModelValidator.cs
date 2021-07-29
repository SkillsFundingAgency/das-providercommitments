using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class DatalockConfirmRestartViewModelValidator :AbstractValidator<DatalockConfirmRestartViewModel>
    {
        public DatalockConfirmRestartViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();
            RuleFor(x => x.SendRequestToEmployer).NotNull().WithMessage("Confirm if you want the employer to make these changes");
        }
    }
}
