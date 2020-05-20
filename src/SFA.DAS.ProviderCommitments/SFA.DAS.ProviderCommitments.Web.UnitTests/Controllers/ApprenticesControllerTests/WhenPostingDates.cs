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

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenPostingDates
    {
        private PostDatesFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new PostDatesFixture();
        }

        [Test]
        public async Task ThenModelMapperIsCalled()
        {
            await _fixture.Act();

            _fixture.VerifyModelMapperWasCalled(Times.Once());
        }

        [Test]
        public async Task ThenRedirectsToPriceRoute()
        {
            var result = await _fixture.Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName(nameof(ApprenticeController.Price));
        }

        [Test]
        public async Task ThenRedirectsToConfirmationRouteWhenInEditMode()
        {
            _fixture.SetEditModeOn();
            var result = await _fixture.Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName(nameof(ApprenticeController.Confirm));
        }
    }

    internal class PostDatesFixture
    {
        private readonly Mock<ICookieStorageService<IndexRequest>> _cookieStorageServiceMock;
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly PriceRequest _request;
        private readonly ApprenticeController _sut;
        private readonly DatesViewModel _viewModel;

        public PostDatesFixture()
        {
            _viewModel = new DatesViewModel
            {
                ApprenticeshipHashedId = "DF34WG2",
                EmployerAccountLegalEntityPublicHashedId = "DFF41G",
                ProviderId = 2342,
                StartDate = new MonthYearModel("62020"),
                StopDate = DateTime.UtcNow.AddDays(-5)
            };
            _request = new PriceRequest
            {
                ApprenticeshipHashedId = _viewModel.ApprenticeshipHashedId,
                EmployerAccountLegalEntityPublicHashedId = _viewModel.EmployerAccountLegalEntityPublicHashedId,
                ProviderId = _viewModel.ProviderId,
                StartDate = _viewModel.StartDate.MonthYear
            };
            _cookieStorageServiceMock = new Mock<ICookieStorageService<IndexRequest>>();
            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock
                .Setup(x => x.Map<PriceRequest>(_viewModel))
                .ReturnsAsync(_request);
            _sut = new ApprenticeController(_modelMapperMock.Object, _cookieStorageServiceMock.Object, Mock.Of<ICommitmentsApiClient>());
        }

        public Task<IActionResult> Act() => _sut.Dates(_viewModel);

        public PostDatesFixture SetEditModeOn()
        {
            _viewModel.Price = 1;
            return this;
        }


        public void VerifyModelMapperWasCalled(Times times) =>
            _modelMapperMock.Verify(x => x.Map<PriceRequest>(_viewModel), times);
    }
}