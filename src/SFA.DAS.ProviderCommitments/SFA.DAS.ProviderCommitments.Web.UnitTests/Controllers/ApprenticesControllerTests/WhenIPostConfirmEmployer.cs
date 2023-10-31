using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenIPostConfirmEmployer
    {
        [Test]
        public async Task PostConfirmEmployerViewModel_WithValidModel_WithConfirmFalse_ShouldRedirectToSelectEmployer()
        {
            var fixture = new PostConfirmEmployerFixture()
                .WithConfirmFalse();

            var result = await fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("SelectEmployer");
        }

        [Test]
        public async Task PostConfirmEmployerViewModel_WithValidModel_WithConfirmTrue_RedirectToDates()
        {
            var fixture = new PostConfirmEmployerFixture()
                .WithConfirmTrue();

            var result = await fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("SelectDeliveryModel");
        }
    }

    public class PostConfirmEmployerFixture
    {
        private readonly ApprenticeController _sut;
        private readonly ConfirmEmployerViewModel _viewModel;

        public PostConfirmEmployerFixture()
        {
            const long providerId = 123;
            _viewModel = new ConfirmEmployerViewModel { ProviderId = providerId, EmployerAccountLegalEntityPublicHashedId = "XYZ" , ApprenticeshipHashedId = "ABC"};
            _sut = new ApprenticeController(Mock.Of<IModelMapper>(), Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        public PostConfirmEmployerFixture WithConfirmFalse()
        {
            _viewModel.Confirm = false;
            return this;
        }

        public PostConfirmEmployerFixture WithConfirmTrue()
        {
            _viewModel.Confirm = true;
            return this;
        }

        public async Task<IActionResult> Act() => await _sut.ConfirmEmployer(_viewModel);
    }
}
