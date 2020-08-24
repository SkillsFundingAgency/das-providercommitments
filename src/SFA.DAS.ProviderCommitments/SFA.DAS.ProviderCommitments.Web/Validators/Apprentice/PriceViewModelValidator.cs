using FluentValidation;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class PriceViewModelValidator : AbstractValidator<PriceViewModel>
    {
        public PriceViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();
            RuleFor(x => x.EmployerAccountLegalEntityPublicHashedId).NotEmpty();
            RuleFor(x => x.StartDate).Must(field => field.IsValidMonthYear());
            RuleFor(x => x.Price).NotEmpty().WithMessage("Enter the new agreed apprenticeship price");
            RuleFor(x => x.Price).GreaterThanOrEqualTo(1).WithMessage("Enter the new agreed apprenticeship price");
            RuleFor(x => x.Price).LessThanOrEqualTo(100000).WithMessage("The new agreed apprenticeship price must be £100,000 or less");
            RuleFor(x => x.Price).Must(x => decimal.ToInt32(x.Value) == x.Value)
                .When(x => x.Price.HasValue)
                .WithMessage("Total agreed apprenticeship price must be 7 numbers or fewer");
        }
    }
}
