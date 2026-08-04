using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class SelectHowManyLearnersToAddViewModelValidator : AbstractValidator<SelectHowManyLearnersToAddViewModel>
    {
        public SelectHowManyLearnersToAddViewModelValidator()
        {
            RuleFor(x => x.Selection).NotNull().WithMessage("Select how many learners you would like to add");
        }
    }
}