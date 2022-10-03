using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.Validators.OverlappingTrainingDate
{
    public class DraftApprenticeshipOverlapOptionWithPendingRequestViewModelValidator : AbstractValidator<DraftApprenticeshipOverlapOptionWithPendingRequestViewModel>
    {
        public DraftApprenticeshipOverlapOptionWithPendingRequestViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.OverlapOptions).NotNull().WithMessage("You need to select whether you want to save the changes");
        }
    }
}
