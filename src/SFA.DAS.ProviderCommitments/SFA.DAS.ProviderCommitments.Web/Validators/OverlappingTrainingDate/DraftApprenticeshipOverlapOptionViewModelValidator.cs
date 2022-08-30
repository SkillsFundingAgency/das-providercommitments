using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.Validators.OverlappingTrainingDate
{
    public class EmployerNotifiedViewModelValidator : AbstractValidator<EmployerNotifiedViewModel>
    {
        public EmployerNotifiedViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.NextAction).NotNull().WithMessage("You need to select what you want to do next");
        }
    }
}
