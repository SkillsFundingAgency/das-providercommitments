using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.Validators.OverlappingTrainingDate
{
    public class OverlapOptionsForChangeEmployerRequestValidator : AbstractValidator<OverlapOptionsForChangeEmployerRequest>
    {
        public OverlapOptionsForChangeEmployerRequestValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.CacheKey).NotNull();
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();

        }
    }
}
