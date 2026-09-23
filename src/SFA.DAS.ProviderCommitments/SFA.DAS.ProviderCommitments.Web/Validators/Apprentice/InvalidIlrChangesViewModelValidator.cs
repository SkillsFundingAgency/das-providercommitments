using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;

public static class UnacknowledgedApprovalChangesValidation
{
    public const string SelectDeleteMessage = "Select if you would like to delete this alert";
}

public abstract class UnacknowledgedApprovalChangesViewModelValidator<T> : AbstractValidator<T>
    where T : InvalidIlrChangesViewModel
{
    protected UnacknowledgedApprovalChangesViewModelValidator()
    {
        RuleForEach(x => x.RequestSets).ChildRules(set =>
        {
            set.RuleFor(x => x.DeleteAlert)
                .NotNull()
                .WithMessage(UnacknowledgedApprovalChangesValidation.SelectDeleteMessage);
        });
    }
}

public class InvalidIlrChangesViewModelValidator : UnacknowledgedApprovalChangesViewModelValidator<InvalidIlrChangesViewModel>
{
    public const string SelectDeleteMessage = UnacknowledgedApprovalChangesValidation.SelectDeleteMessage;
}

public class DeclinedChangesViewModelValidator : UnacknowledgedApprovalChangesViewModelValidator<DeclinedChangesViewModel>
{
}
