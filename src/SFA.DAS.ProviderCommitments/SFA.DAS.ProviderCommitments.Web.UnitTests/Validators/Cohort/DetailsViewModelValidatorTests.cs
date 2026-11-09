using System;
using System.Linq.Expressions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort;

[TestFixture]
public class DetailsViewModelValidatorTests
{
    [TestCase(null, false)]
    [TestCase(CohortDetailsOptions.Send, true)]
    [TestCase(CohortDetailsOptions.Approve, true)]
    public void Validate_Selection_ShouldBeValidated(CohortDetailsOptions? selection, bool expectedValid)
    {
        var model = new DetailsViewModel { Selection = selection };
        AssertValidationResult(request => request.Selection, model, expectedValid);
    }

    private static void AssertValidationResult<T>(Expression<Func<DetailsViewModel, T>> property, DetailsViewModel instance, bool expectedValid)
    {
        var validator = new DetailsViewModelValidator();
        var result = validator.TestValidate(instance);

        if (expectedValid)
        {
            result.ShouldNotHaveValidationErrorFor(property);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(property);
        }
    }

    [TestCase(true, CohortDetailsOptions.Send, true)]
    [TestCase(true, CohortDetailsOptions.Approve, true)]
    [TestCase(true, CohortDetailsOptions.ApprenticeRequest, true)]
    [TestCase(false, CohortDetailsOptions.Send, false)]
    [TestCase(false, CohortDetailsOptions.Approve, false)]
    [TestCase(false, CohortDetailsOptions.ApprenticeRequest, true)]
    public void Validate_RplVerified_ShouldBeValidated(bool rplVerified, CohortDetailsOptions? selection, bool expectedValid)
    {
        var model = new DetailsViewModel
        {
            RplVerified = rplVerified,
            Selection = selection,
            ShowRplConfirmation = true
        };

        AssertValidationResult(request => request.RplVerified, model, expectedValid);
    }

    [TestCase(false, CohortDetailsOptions.Send)]
    [TestCase(false, CohortDetailsOptions.Approve)]
    public void Validate_RplVerified_ShouldNotHaveValidationError_WhenShowRplConfirmationIsFalse(
    bool rplVerified,
    CohortDetailsOptions? selection)
    {
        var model = new DetailsViewModel
        {
            RplVerified = rplVerified,
            Selection = selection,
            ShowRplConfirmation = false
        };

        AssertValidationResult(request => request.RplVerified, model, true);
    }

    [Test]
    public void ThenDoesNotRequireRplConfirmationWhenCohortContainsOnlyApprenticeshipUnits()
    {
        var fixture = new WhenValidatingRplConfirmationFixture()
            .WithShowRplConfirmation(false);

        var result = fixture.Act();

        result.ShouldNotHaveValidationErrorFor(x => x.RplVerified);
    }

    [Test]
    public void ThenRequiresRplConfirmationWhenCohortContainsMixedApprenticeshipsAndApprenticeshipUnits()
    {
        var fixture = new WhenValidatingRplConfirmationFixture()
            .WithShowRplConfirmation(true);

        var result = fixture.Act();

        result.ShouldHaveValidationErrorFor(x => x.RplVerified)
            .WithErrorMessage("Check the box to confirm you have checked RPL for each learner");
    }
}

public class WhenValidatingRplConfirmationFixture
{
    private readonly DetailsViewModelValidator _validator;
    private readonly DetailsViewModel _viewModel;

    public WhenValidatingRplConfirmationFixture()
    {
        _validator = new DetailsViewModelValidator();

        _viewModel = new DetailsViewModel
        {
            Selection = CohortDetailsOptions.Send,
            RplVerified = false
        };
    }

    public WhenValidatingRplConfirmationFixture WithShowRplConfirmation(bool show)
    {
        _viewModel.ShowRplConfirmation = show;
        return this;
    }

    public WhenValidatingRplConfirmationFixture WithRplVerified(bool verified)
    {
        _viewModel.RplVerified = verified;
        return this;
    }

    public TestValidationResult<DetailsViewModel> Act()
    {
        return _validator.TestValidate(_viewModel);
    }
}