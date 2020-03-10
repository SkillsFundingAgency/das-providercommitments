using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class ChangePriceViewModelValidator : AbstractValidator<PriceViewModel>
    {
        public ChangePriceViewModelValidator()
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
                .WithMessage("The value {PropertyValue} is not valid for the new agreed apprenticeship price");
        }
    }
}
