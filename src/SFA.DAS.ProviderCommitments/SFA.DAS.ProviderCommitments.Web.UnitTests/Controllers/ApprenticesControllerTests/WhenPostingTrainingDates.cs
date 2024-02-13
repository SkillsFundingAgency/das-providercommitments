using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenPostingTrainingDates
    {
        private PostTrainingDatesFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new PostTrainingDatesFixture();
        }

        [Test]
        public async Task ThenCallsModelMapper()
        {
            await _fixture.Act();

            _fixture.Verify_ModelMapperWasCalled(Times.Once());
        }

        [Test]
        public async Task PostTrainingDatesViewModel_ShouldRedirectToPrice()
        {
            var result = await _fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName(nameof(ApprenticeController.Price));
        }
    }

    public class PostTrainingDatesFixture
    {
        private ApprenticeController Sut { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly PriceRequest _request;
        private readonly TrainingDatesViewModel _viewModel;
        private Mock<IOuterApiService> _mockOuterApiService;

        public PostTrainingDatesFixture()
        {
            _request = new PriceRequest
            {
                ProviderId = 2342,
                ApprenticeshipHashedId = "KG34DF989"
            };
            _viewModel = new TrainingDatesViewModel
            {
                ProviderId = 2342,
                CacheKey = Guid.NewGuid()
            };

            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock
                .Setup(x => x.Map<PriceRequest>(_viewModel))
                .ReturnsAsync(_request);

            _mockOuterApiService = new Mock<IOuterApiService>();

            _mockOuterApiService.Setup(x =>
                    x.ValidateChangeOfEmployerOverlap(It.IsAny<ValidateChangeOfEmployerOverlapApimRequest>()))
                .Returns(Task.CompletedTask);

            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(),
                Mock.Of<ICommitmentsApiClient>(), _mockOuterApiService.Object, Mock.Of<ICacheStorageService>());
        }

        public void Verify_ModelMapperWasCalled(Times times) =>
            _modelMapperMock.Verify(x => x.Map<PriceRequest>(_viewModel), times);

        public async Task<IActionResult> Act() => await Sut.TrainingDates(_viewModel);
    }
}