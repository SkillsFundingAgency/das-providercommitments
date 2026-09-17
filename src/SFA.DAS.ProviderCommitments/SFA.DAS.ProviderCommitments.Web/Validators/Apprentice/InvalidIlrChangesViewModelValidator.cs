using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;

public class InvalidIlrChangesViewModelValidator : AbstractValidator<InvalidIlrChangesViewModel>
{
    public const string SelectDeleteMessage = "Select if you would like to delete this alert";

    public InvalidIlrChangesViewModelValidator()
    {
        RuleFor(x => x.RequestSets).Custom((requestSets, context) =>
        {
            if (requestSets == null)
            {
                return;
            }

            for (var index = 0; index < requestSets.Count; index++)
            {
                var requestSet = requestSets[index];
                if (requestSet.DeleteAlert != null)
                {
                    continue;
                }

                context.AddFailure(
                    $"RequestSets[{index}].DeleteAlert",
                    SelectDeleteMessageFor(requestSet));
            }
        });
    }

    public static string SelectDeleteMessageFor(InvalidIlrChangeSetViewModel requestSet)
    {
        return $"{SelectDeleteMessage} for {GetAlertCaption(requestSet)}";
    }

    public static string GetAlertCaption(InvalidIlrChangeSetViewModel requestSet)
    {
        if (requestSet?.Fields?.Count == 1)
        {
            return requestSet.Fields[0].FieldDisplayName;
        }

        return "Invalid ILR changes";
    }
}
