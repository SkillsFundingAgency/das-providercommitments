using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class ChangePriceRequestValidator : AbstractValidator<ChangePriceRequest>
    {
        public ChangePriceRequestValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();
            RuleFor(x => x.EmployerAccountLegalEntityPublicHashedId).NotEmpty(); 
            RuleFor(x => x.NewStartDate)
                .Must(field => field.IsValidMonthYear())
                .WithMessage("{PropertyName} is invalid format");
        }
    }
}
