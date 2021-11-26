using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class SelectDraftApprenticeshipsEntryMethodViewModelValidator : AbstractValidator<SelectAddDraftApprenticeshipJourneyViewModel>
    {
        public SelectDraftApprenticeshipsEntryMethodViewModelValidator()
        {
            RuleFor(x => x.Selection).NotNull().WithMessage("Select how you want to add apprentices");
        }
    }
}