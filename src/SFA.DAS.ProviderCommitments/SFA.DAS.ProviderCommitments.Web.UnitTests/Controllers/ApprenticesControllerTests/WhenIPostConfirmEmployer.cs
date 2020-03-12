using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticeControllerTests
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
            result.VerifyReturnsRedirectToActionResult().WithActionName("Dates");
        }
    }

    public class PostConfirmEmployerFixture
    {
        public ApprenticeController Sut { get; set; }

        public string RedirectUrl;
        private readonly ConfirmEmployerViewModel _viewModel;
        private readonly long _providerId;

        public PostConfirmEmployerFixture()
        {
            _providerId = 123;
            _viewModel = new ConfirmEmployerViewModel { ProviderId = _providerId, EmployerAccountLegalEntityPublicHashedId = "XYZ" , ApprenticeshipHashedId = "ABC"};
            Sut = new ApprenticeController(Mock.Of<IModelMapper>(), Mock.Of<ICookieStorageService<IndexRequest>>());
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

        public async Task<IActionResult> Act() => Sut.ConfirmEmployer(_viewModel);
    }
}
