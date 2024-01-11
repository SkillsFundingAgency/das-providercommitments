using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.Validators.OverlappingTrainingDate;

public class ChangeOfEmployerNotifiedViewModelValidator: AbstractValidator<ChangeOfEmployerNotifiedViewModel>
{
    public ChangeOfEmployerNotifiedViewModelValidator()
    {
        RuleFor(x => x.ProviderId).GreaterThan(0);
        RuleFor(x => x.NextAction).NotNull().WithMessage("You need to select what you would like to do");
    }
}