using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Validators.DraftApprenticeship;

public class SelectAddAnotherDraftApprenticeshipJourneyViewModelValidator : AbstractValidator<SelectAddAnotherDraftApprenticeshipJourneyViewModel>
{
    public SelectAddAnotherDraftApprenticeshipJourneyViewModelValidator()
    {
        RuleFor(x => x.Selection).NotNull().WithMessage("You need to select how you want to add apprentice details");
    }
}
