using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;

public class InvalidIlrChangesViewModelValidator : AbstractValidator<InvalidIlrChangesViewModel>
{
    public const string SelectDeleteMessage = "Select if you would like to delete this notification and alert";

    public InvalidIlrChangesViewModelValidator()
    {
        RuleForEach(x => x.RequestSets).ChildRules(set =>
        {
            set.RuleFor(x => x.DeleteAlert)
                .NotNull()
                .WithMessage(SelectDeleteMessage);
        });
    }
}
