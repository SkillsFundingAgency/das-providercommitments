using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class DataLockRequestRestartViewModelValidator : AbstractValidator<DataLockRequestRestartViewModel>
    {
        public DataLockRequestRestartViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();
            RuleFor(x => x.SubmitStatusViewModel).NotNull().WithMessage("Confirm how you want to fix this mismatch");
        }
    }
}
