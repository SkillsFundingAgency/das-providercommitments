using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Helpers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingRPLCheckboxVisibility
    {
        [Test]
        public void ThenHidesCheckboxWhenAllLearnersAreApprenticeshipUnits()
        {
            var fixture = new WhenGettingRPLCheckboxVisibilityFixture()
                .WithApprenticeshipTypes(
                    ApprenticeshipType.ApprenticeshipUnit,
                    ApprenticeshipType.ApprenticeshipUnit);

            var result = fixture.Act();

            result.Should().BeFalse();
        }

        [Test]
        public void ThenDisplaysCheckboxWhenLearnersAreRegularApprenticeships()
        {
            var fixture = new WhenGettingRPLCheckboxVisibilityFixture()
                .WithApprenticeshipTypes(
                    ApprenticeshipType.Apprenticeship,
                    ApprenticeshipType.Apprenticeship);

            var result = fixture.Act();

            result.Should().BeTrue();
        }

        [Test]
        public void ThenDisplaysCheckboxWhenLearnersAreFoundationApprenticeships()
        {
            var fixture = new WhenGettingRPLCheckboxVisibilityFixture()
                .WithApprenticeshipTypes(
                    ApprenticeshipType.FoundationApprenticeship,
                    ApprenticeshipType.FoundationApprenticeship);

            var result = fixture.Act();

            result.Should().BeTrue();
        }

        [Test]
        public void ThenDisplaysCheckboxWhenCohortContainsApprenticeshipsAndApprenticeshipUnits()
        {
            var fixture = new WhenGettingRPLCheckboxVisibilityFixture()
                .WithApprenticeshipTypes(
                    ApprenticeshipType.Apprenticeship,
                    ApprenticeshipType.ApprenticeshipUnit);

            var result = fixture.Act();

            result.Should().BeTrue();
        }

        [Test]
        public void ThenDisplaysCheckboxWhenCohortContainsFoundationApprenticeshipsAndApprenticeshipUnits()
        {
            var fixture = new WhenGettingRPLCheckboxVisibilityFixture()
                .WithApprenticeshipTypes(
                    ApprenticeshipType.FoundationApprenticeship,
                    ApprenticeshipType.ApprenticeshipUnit);

            var result = fixture.Act();

            result.Should().BeTrue();
        }

        [Test]
        public void ThenDisplaysCheckboxWhenCohortContainsAllApprenticeshipTypes()
        {
            var fixture = new WhenGettingRPLCheckboxVisibilityFixture()
                .WithApprenticeshipTypes(
                    ApprenticeshipType.Apprenticeship,
                    ApprenticeshipType.FoundationApprenticeship,
                    ApprenticeshipType.ApprenticeshipUnit);

            var result = fixture.Act();

            result.Should().BeTrue();
        }

        [Test]
        public void ThenDisplaysCheckboxWhenLearnerTypeIsUnknown()
        {
            var fixture = new WhenGettingRPLCheckboxVisibilityFixture()
                .WithApprenticeshipTypes(
                    ApprenticeshipType.Unknown);

            var result = fixture.Act();

            result.Should().BeTrue();
        }

        [Test]
        public void ThenHidesCheckboxWhenThereAreNoLearners()
        {
            var fixture = new WhenGettingRPLCheckboxVisibilityFixture();

            var result = fixture.Act();

            result.Should().BeFalse();
        }
    }

    [TestFixture]
    public class WhenValidatingRplConfirmation
    {
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

    public class WhenGettingRPLCheckboxVisibilityFixture
    {
        private IEnumerable<ApprenticeshipType> _apprenticeshipTypes =
            Enumerable.Empty<ApprenticeshipType>();

        public WhenGettingRPLCheckboxVisibilityFixture WithApprenticeshipTypes(params ApprenticeshipType[] apprenticeshipTypes)
        {
            _apprenticeshipTypes = apprenticeshipTypes;

            return this;
        }

        public bool Act()
        {
            return _apprenticeshipTypes.Any(at => at != ApprenticeshipType.ApprenticeshipUnit);
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
}