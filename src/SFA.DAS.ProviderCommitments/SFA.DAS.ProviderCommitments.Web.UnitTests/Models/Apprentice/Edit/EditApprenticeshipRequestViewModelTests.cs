using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.Apprentice.Edit
{
    [TestFixture]
    public class EditApprenticeshipRequestViewModelTests
    {
        private EditApprenticeshipRequestViewModel _viewModel;

        [SetUp]
        public void Arrange()
        {
            _viewModel = new EditApprenticeshipRequestViewModel();
        }

        [Test]
        public void IsStartDateManualEditLockedForUpdate_WhenNotLocked_ReturnsFalse()
        {
            _viewModel.IsStartDateManualEditLockedForUpdate.Should().BeFalse();
        }

        [TestCase(nameof(EditApprenticeshipRequestViewModel.IsLockedForUpdate))]
        [TestCase(nameof(EditApprenticeshipRequestViewModel.IsUpdateLockedForStartDateAndCourse))]
        public void IsStartDateManualEditLockedForUpdate_WhenBaseLockIsSet_ReturnsTrue(string propertyName)
        {
            typeof(EditApprenticeshipRequestViewModel).GetProperty(propertyName).SetValue(_viewModel, true);

            _viewModel.IsStartDateManualEditLockedForUpdate.Should().BeTrue();
        }

        [Test]
        public void IsStartDateManualEditLockedForUpdate_WhenStartDateManualFieldIsLocked_ReturnsTrue()
        {
            _viewModel.LockedManualFieldsAndFeatures = new List<ManualFieldOrFeature> { ManualFieldOrFeature.StartDate };

            _viewModel.IsStartDateManualEditLockedForUpdate.Should().BeTrue();
        }

        [Test]
        public void IsStartDateManualEditLockedForUpdate_WhenOnlyOtherManualFieldsAreLocked_ReturnsFalse()
        {
            _viewModel.LockedManualFieldsAndFeatures = new List<ManualFieldOrFeature> { ManualFieldOrFeature.EndDate, ManualFieldOrFeature.Price, ManualFieldOrFeature.Withdrawal };

            _viewModel.IsStartDateManualEditLockedForUpdate.Should().BeFalse();
        }

        [Test]
        public void IsEndDateManualEditLockedForUpdate_WhenNotLocked_ReturnsFalse()
        {
            _viewModel.IsEndDateManualEditLockedForUpdate.Should().BeFalse();
        }

        [Test]
        public void IsEndDateManualEditLockedForUpdate_WhenEndDateLockedForUpdate_ReturnsTrue()
        {
            typeof(EditApprenticeshipRequestViewModel).GetProperty(nameof(EditApprenticeshipRequestViewModel.IsEndDateLockedForUpdate)).SetValue(_viewModel, true);

            _viewModel.IsEndDateManualEditLockedForUpdate.Should().BeTrue();
        }

        [Test]
        public void IsEndDateManualEditLockedForUpdate_WhenEndDateManualFieldIsLocked_ReturnsTrue()
        {
            _viewModel.LockedManualFieldsAndFeatures = new List<ManualFieldOrFeature> { ManualFieldOrFeature.EndDate };

            _viewModel.IsEndDateManualEditLockedForUpdate.Should().BeTrue();
        }

        [Test]
        public void IsEndDateManualEditLockedForUpdate_WhenOnlyStartDateManualFieldIsLocked_ReturnsFalse()
        {
            _viewModel.LockedManualFieldsAndFeatures = new List<ManualFieldOrFeature> { ManualFieldOrFeature.StartDate };

            _viewModel.IsEndDateManualEditLockedForUpdate.Should().BeFalse();
        }

        [Test]
        public void IsCostManualEditLockedForUpdate_WhenNotLocked_ReturnsFalse()
        {
            _viewModel.IsCostManualEditLockedForUpdate.Should().BeFalse();
        }

        [Test]
        public void IsCostManualEditLockedForUpdate_WhenIsLockedForUpdate_ReturnsTrue()
        {
            _viewModel.IsLockedForUpdate = true;

            _viewModel.IsCostManualEditLockedForUpdate.Should().BeTrue();
        }

        [Test]
        public void IsCostManualEditLockedForUpdate_WhenPriceManualFieldIsLocked_ReturnsTrue()
        {
            _viewModel.LockedManualFieldsAndFeatures = new List<ManualFieldOrFeature> { ManualFieldOrFeature.Price };

            _viewModel.IsCostManualEditLockedForUpdate.Should().BeTrue();
        }

        [Test]
        public void IsCostManualEditLockedForUpdate_WhenOnlyOtherManualFieldsAreLocked_ReturnsFalse()
        {
            _viewModel.LockedManualFieldsAndFeatures = new List<ManualFieldOrFeature> { ManualFieldOrFeature.StartDate, ManualFieldOrFeature.EndDate, ManualFieldOrFeature.Withdrawal };

            _viewModel.IsCostManualEditLockedForUpdate.Should().BeFalse();
        }

        [Test]
        public void IsCostManualEditLockedForUpdate_WhenLockedManualFieldsIsNull_ReturnsFalse()
        {
            _viewModel.LockedManualFieldsAndFeatures = null;

            _viewModel.IsCostManualEditLockedForUpdate.Should().BeFalse();
        }

        [Test]
        public void IsCostManualEditLockedForUpdate_WhenIsStartDateLockedOnly_ReturnsFalse()
        {
            _viewModel.IsUpdateLockedForStartDateAndCourse = true;

            _viewModel.IsCostManualEditLockedForUpdate.Should().BeFalse();
        }
    }
}