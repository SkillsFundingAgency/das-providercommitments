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
                new InvalidIlrChangeSetViewModel { DeleteAlert = null },
                new InvalidIlrChangeSetViewModel { DeleteAlert = true }
            ]
        };

        var result = new InvalidIlrChangesViewModelValidator().TestValidate(viewModel);

        result.ShouldHaveValidationErrorFor("RequestSets[0].DeleteAlert")
            .WithErrorMessage(InvalidIlrChangesViewModelValidator.SelectDeleteMessage);
        result.ShouldNotHaveValidationErrorFor("RequestSets[1].DeleteAlert");
    }

    [Test]
    public void Validate_ThenAddsOneErrorPerMissingAnswer()
    {
        var viewModel = new InvalidIlrChangesViewModel
        {
            RequestSets =
            [
                new InvalidIlrChangeSetViewModel { DeleteAlert = null },
                new InvalidIlrChangeSetViewModel { DeleteAlert = null }
            ]
        };

        var result = new InvalidIlrChangesViewModelValidator().TestValidate(viewModel);

        result.ShouldHaveValidationErrorFor("RequestSets[0].DeleteAlert")
            .WithErrorMessage(InvalidIlrChangesViewModelValidator.SelectDeleteMessage);
        result.ShouldHaveValidationErrorFor("RequestSets[1].DeleteAlert")
            .WithErrorMessage(InvalidIlrChangesViewModelValidator.SelectDeleteMessage);
        result.Errors.Should().HaveCount(2);
    }

    [Test]
    public void Validate_ThenDeclinedChangesUsesTheSameRequiredRadioRule()
    {
        var viewModel = new DeclinedChangesViewModel
        {
            RequestSets =
            [
                new InvalidIlrChangeSetViewModel { DeleteAlert = null }
            ]
        };

        var result = new DeclinedChangesViewModelValidator().TestValidate(viewModel);

        result.ShouldHaveValidationErrorFor("RequestSets[0].DeleteAlert")
            .WithErrorMessage(InvalidIlrChangesViewModelValidator.SelectDeleteMessage);
    }
}
