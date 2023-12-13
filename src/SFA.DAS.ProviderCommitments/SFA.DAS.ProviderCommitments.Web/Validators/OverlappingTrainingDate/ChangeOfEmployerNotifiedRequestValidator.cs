using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.Validators.OverlappingTrainingDate;

public class ChangeOfEmployerNotifiedRequestValidator : AbstractValidator<ChangeOfEmployerNotifiedRequest>
{
    public ChangeOfEmployerNotifiedRequestValidator()
    {
        RuleFor(x => x.ProviderId).GreaterThan(0);
    }
}