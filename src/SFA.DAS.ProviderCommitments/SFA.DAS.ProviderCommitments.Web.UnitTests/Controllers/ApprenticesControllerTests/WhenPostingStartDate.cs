using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenPostingStartDate
    {
        private PostStartDateFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new PostStartDateFixture();
        }

        [Test]
        public async Task ThenModelMapperIsCalled()
        {
            await _fixture.Act();

            _fixture.VerifyModelMapperWasCalled(Times.Once());
        }

        [Test]
        public async Task ThenRedirectsToEndDateRoute()
        {
            var result = await _fixture.Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName(nameof(ApprenticeController.EndDate));
        }

        [Test]
        public async Task ThenRedirectsToConfirmationRouteWhenInEditMode()
        {
            _fixture.SetEditModeOn();
            var result = await _fixture.Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName(nameof(ApprenticeController.Confirm));
        }
    }

    internal class PostStartDateFixture
    {
        private readonly Mock<ICookieStorageService<IndexRequest>> _cookieStorageServiceMock;
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly EndDateRequest _request;
        private readonly ApprenticeController _sut;
        private readonly StartDateViewModel _viewModel;

        public PostStartDateFixture()
        {
            _viewModel = new StartDateViewModel
            {
                ApprenticeshipHashedId = "DF34WG2",
                ProviderId = 2342,
                StartDate = new MonthYearModel("62020"),
                StopDate = DateTime.UtcNow.AddDays(-5)
            };

            _request = new EndDateRequest
            {
                ApprenticeshipHashedId = _viewModel.ApprenticeshipHashedId,
                ProviderId = _viewModel.ProviderId
            };

            _cookieStorageServiceMock = new Mock<ICookieStorageService<IndexRequest>>();
            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock
                .Setup(x => x.Map<EndDateRequest>(_viewModel))
                .ReturnsAsync(_request);
            _sut = new ApprenticeController(_modelMapperMock.Object, _cookieStorageServiceMock.Object, Mock.Of<ICommitmentsApiClient>());
        }

        public Task<IActionResult> Act() => _sut.StartDate(_viewModel);

        public PostStartDateFixture SetEditModeOn()
        {
            _viewModel.InEditMode = true;
            return this;
        }

        public void VerifyModelMapperWasCalled(Times times) =>
            _modelMapperMock.Verify(x => x.Map<EndDateRequest>(_viewModel), times);
    }
}