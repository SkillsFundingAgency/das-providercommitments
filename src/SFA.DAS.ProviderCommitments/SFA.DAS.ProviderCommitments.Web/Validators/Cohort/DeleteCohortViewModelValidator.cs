using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class DeleteCohortViewModelValidator : AbstractValidator<DeleteCohortViewModel>
    {
        public DeleteCohortViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.CohortReference).NotNull();
            RuleFor(x => x.Confirm).NotNull().WithMessage("Confirm if you would like to delete this cohort");
        }
    }
}
