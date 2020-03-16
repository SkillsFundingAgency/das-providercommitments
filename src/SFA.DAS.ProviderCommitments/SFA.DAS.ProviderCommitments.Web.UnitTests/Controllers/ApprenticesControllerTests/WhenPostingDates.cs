using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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
        public async Task AndNoValidationErrorThrown_ThenRedirectsToPrices()
        {
            var result = await _fixture.Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName(nameof(ApprenticeController.ChangePrice));
        }

        [Test]
        public async Task AndValidationExceptionIsThrown_ThenRedirectsToDates()
        {
            _fixture.WithValidationError();

            var result = await _fixture.Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName(nameof(ApprenticeController.Dates));
        }
    }

    internal class PostDatesFixture
    {
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly PriceRequest _request;
        private readonly ApprenticeController _sut;
        private readonly DatesViewModel _viewModel;

        public PostDatesFixture()
        {
            _request = new PriceRequest
            {
                ApprenticeshipHashedId = "DFO24FD",
                EmployerAccountLegalEntityPublicHashedId = "DFE3434DF",
                ProviderId = 32957,
                NewStartDate = "62020"
            };
            _viewModel = new DatesViewModel
            {
                ApprenticeshipHashedId = "DF34WG2",
                EmployerAccountLegalEntityPublicHashedId = "DFF41G",
                ProviderId = 2342,
                StartDate = new MonthYearModel("62020"),
                StopDate = DateTime.UtcNow.AddDays(-5)
            };
            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock
                .Setup(x => x.Map<PriceRequest>(_viewModel))
                .ReturnsAsync(_request);

            _sut = new ApprenticeController(_modelMapperMock.Object);
        }

        public Task<IActionResult> Act() => _sut.Dates(_viewModel);

        public PostDatesFixture WithValidationError()
        {
            _modelMapperMock
                .Setup(x => x.Map<PriceRequest>(_viewModel))
                .ThrowsAsync(new ValidationException());
            return this;
        }

        public void VerifyModelMapperWasCalled(Times times) =>
            _modelMapperMock.Verify(x => x.Map<PriceRequest>(_viewModel), times);
    }
}