using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class SelectDraftApprenticeshipsEntryMethodViewModelValidator : AbstractValidator<SelectDraftApprenticeshipsEntryMethodViewModel>
    {
        public SelectDraftApprenticeshipsEntryMethodViewModelValidator()
        {
            RuleFor(x => x.Selection).NotNull().WithMessage("Select how you want to add apprentices");
        }
    }
}