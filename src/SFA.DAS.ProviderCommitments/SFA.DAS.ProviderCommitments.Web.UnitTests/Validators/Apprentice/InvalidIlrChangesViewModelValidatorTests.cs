using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice;

public class InvalidIlrChangesViewModelValidatorTests
{
    [Test]
    public void Validate_ThenIsValidWhenEveryRadioIsAnswered()
    {
        var viewModel = new InvalidIlrChangesViewModel
        {
            RequestSets =
            [
                new InvalidIlrChangeSetViewModel { DeleteAlert = true },
                new InvalidIlrChangeSetViewModel { DeleteAlert = false }
            ]
        };

        var result = new InvalidIlrChangesViewModelValidator().TestValidate(viewModel);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_ThenRequiresAnAnswerForEachRequestSet()
    {
        var viewModel = new InvalidIlrChangesViewModel
        {
            RequestSets =
            [
                new InvalidIlrChangeSetViewModel
                {
                    DeleteAlert = null,
                    Fields = [new InvalidIlrChangeFieldViewModel { FieldDisplayName = "Total price" }]
                },
                new InvalidIlrChangeSetViewModel
                {
                    DeleteAlert = true,
                    Fields = [new InvalidIlrChangeFieldViewModel { FieldDisplayName = "Date of birth" }]
                }
            ]
        };

        var result = new InvalidIlrChangesViewModelValidator().TestValidate(viewModel);

        result.ShouldHaveValidationErrorFor("RequestSets[0].DeleteAlert")
            .WithErrorMessage("Select if you would like to delete this alert for Total price");
        result.ShouldNotHaveValidationErrorFor("RequestSets[1].DeleteAlert");
    }

    [Test]
    public void Validate_ThenUsesDistinctMessagesForEachMissingAnswer()
    {
        var viewModel = new InvalidIlrChangesViewModel
        {
            RequestSets =
            [
                new InvalidIlrChangeSetViewModel
                {
                    DeleteAlert = null,
                    Fields = [new InvalidIlrChangeFieldViewModel { FieldDisplayName = "Total price" }]
                },
                new InvalidIlrChangeSetViewModel
                {
                    DeleteAlert = null,
                    Fields = [new InvalidIlrChangeFieldViewModel { FieldDisplayName = "Date of birth" }]
                }
            ]
        };

        var result = new InvalidIlrChangesViewModelValidator().TestValidate(viewModel);

        result.ShouldHaveValidationErrorFor("RequestSets[0].DeleteAlert")
            .WithErrorMessage("Select if you would like to delete this alert for Total price");
        result.ShouldHaveValidationErrorFor("RequestSets[1].DeleteAlert")
            .WithErrorMessage("Select if you would like to delete this alert for Date of birth");
    }

    [Test]
    public void GetAlertCaption_ThenUsesFieldName()
    {
        var requestSet = new InvalidIlrChangeSetViewModel
        {
            Fields = [new InvalidIlrChangeFieldViewModel { FieldDisplayName = "Total price" }]
        };

        InvalidIlrChangesViewModelValidator.GetAlertCaption(requestSet)
            .Should().Be("Total price");
    }

    [Test]
    public void SelectDeleteMessageFor_ThenUsesFieldName()
    {
        var requestSet = new InvalidIlrChangeSetViewModel
        {
            Fields = [new InvalidIlrChangeFieldViewModel { FieldDisplayName = "Total price" }]
        };

        InvalidIlrChangesViewModelValidator.SelectDeleteMessageFor(requestSet)
            .Should().Be("Select if you would like to delete this alert for Total price");
    }
}
