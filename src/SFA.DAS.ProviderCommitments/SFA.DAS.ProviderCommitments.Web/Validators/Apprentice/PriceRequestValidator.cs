using FluentValidation;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class PriceRequestValidator : AbstractValidator<PriceRequest>
    {
        public PriceRequestValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();
            RuleFor(x => x.EmployerAccountLegalEntityPublicHashedId).NotEmpty();
            RuleFor(x => x.StartDate).Must(field => field.IsValidMonthYear());
        }
    }
}
