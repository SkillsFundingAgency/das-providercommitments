using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenPostingSelectDeliveryModel
    {
        [Test]
        public async Task PostSelectDeliveryModelModel_WithIsEdit_ShouldRedirectToConfirm()
        {
            var fixture = new PostSelectDeliveryModelFixture()
                .WithIsEditTrue();

            var result = await fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName(nameof(ApprenticeController.Confirm));
        }

        [Test]
        public async Task PostSelectDeliveryModelModel_WithApprenticeshipStatusStopped_ShouldRedirectToStartDate()
        {
            var fixture = new PostSelectDeliveryModelFixture();
            fixture = fixture.WithApprenticeshipStatus(ApprenticeshipStatus.Stopped);

            var result = await fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName(nameof(ApprenticeController.StartDate));
        }

        [Test]
        public async Task PostSelectDeliveryModelModel_WithApprenticeshipStatusLive_ShouldRedirectToTrainingDates()
        {
            var fixture = new PostSelectDeliveryModelFixture();
            fixture = fixture.WithApprenticeshipStatus(ApprenticeshipStatus.Live);

            var result = await fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName(nameof(ApprenticeController.TrainingDates));
        }
    }

    public class PostSelectDeliveryModelFixture
    {
        public ApprenticeController Sut { get; set; }

        private readonly SelectDeliveryModelViewModel _viewModel;
        private readonly long _providerId;

        public PostSelectDeliveryModelFixture()
        {
            _providerId = 123;
            _viewModel = new SelectDeliveryModelViewModel
            {
                ProviderId = _providerId, EmployerAccountLegalEntityPublicHashedId = "XYZ",
                ApprenticeshipHashedId = "ABC"
            };
            Sut = new ApprenticeController(Mock.Of<IModelMapper>(), Mock.Of<ICookieStorageService<IndexRequest>>(),
                Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>());
        }

        public PostSelectDeliveryModelFixture WithIsEditTrue()
        {
            _viewModel.IsEdit = true;
            return this;
        }

        public PostSelectDeliveryModelFixture WithApprenticeshipStatus(ApprenticeshipStatus status)
        {
            _viewModel.ApprenticeshipStatus = status;
            return this;
        }

        public async Task<IActionResult> Act() => await Sut.SelectDeliveryModel(_viewModel);
    }
}