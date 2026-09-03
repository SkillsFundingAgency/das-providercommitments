using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.Validators.OverlappingTrainingDate
{
    public class OverlapOptionsForChangeEmployerViewModelValidator : AbstractValidator<OverlapOptionsForChangeEmployerViewModel>
    {
        public OverlapOptionsForChangeEmployerViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0).Unless(x => x.HasWithdrawnStatusCode);
            RuleFor(x => x.CacheKey).NotNull().Unless(x => x.HasWithdrawnStatusCode);
            RuleFor(x => x.ApprenticeshipId).GreaterThan(0).Unless(x => x.HasWithdrawnStatusCode);
            RuleFor(x => x.OverlapOptions).NotNull().Unless(x => x.HasWithdrawnStatusCode).WithMessage("You need to select what you would like to do");
        }
    }
}
