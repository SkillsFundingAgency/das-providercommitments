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

                context.AddFailure($"RequestSets[{index}].DeleteAlert", SelectDeleteMessage);
            }
        });
    }

    public static string GetAlertCaption(InvalidIlrChangeSetViewModel requestSet)
    {
        if (requestSet?.Fields?.Count == 1
            && !string.IsNullOrWhiteSpace(requestSet.Fields[0].FieldDisplayName))
        {
            return requestSet.Fields[0].FieldDisplayName;
        }

        return null;
    }
}
