using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.Validators;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    [TestFixture]
    public class ChangeOptionViewModelValidatorTests
    {
        [Test, MoqAutoData]
        public void When_OptionSelected_Then_ReturnValid(
               ChangeOptionViewModel viewModel,
               ChangeOptionViewModelValidator validator)
        {
            var result = validator.Validate(viewModel);

            Assert.True(result.IsValid);
        }

        [Test, MoqAutoData]
        public void When_SelectedOptionIsNull_Then_ReturnInvalid(
               ChangeOptionViewModel viewModel,
               ChangeOptionViewModelValidator validator)
        {
            viewModel.SelectedOption = null;

            var result = validator.Validate(viewModel);

            Assert.False(result.IsValid);
        }

        [Test, MoqAutoData]
        public void When_OnlyChangingOption_And_CurrentOptionIsSelected_Then_ReturnInvalid(
            ChangeOptionViewModel viewModel,
            ChangeOptionViewModelValidator validator)
        {
            viewModel.ReturnToEdit = false;
            viewModel.ReturnToChangeVersion = false;
            viewModel.SelectedOption = viewModel.CurrentOption;

            var result = validator.Validate(viewModel);

            Assert.False(result.IsValid);
        }
    }
}
