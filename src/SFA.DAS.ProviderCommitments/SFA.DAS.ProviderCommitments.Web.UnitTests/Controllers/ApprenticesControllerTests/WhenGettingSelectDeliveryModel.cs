using System;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using DeliveryModel = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types.DeliveryModel;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingSelectDeliveryModel
    {
        private GetSelectDeliveryModelFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new GetSelectDeliveryModelFixture();
        }

        [Test]
        public async Task ThenCallsModelMapper()
        {
            await _fixture.Act();

            _fixture.Verify_ModelMapperWasCalled(Times.Once());
        }

        [Test]
        public async Task WithDeliveryModel_ThenReturnsView()
        {
            _fixture = _fixture.WithDeliveryModel();
            var result = await _fixture.Act();

            result.VerifyReturnsViewModel().WithModel<SelectDeliveryModelViewModel>();
        }

        [Test]
        public async Task WithoutDeliveryModel_RedirectToTrainingDates()
        {
            var result = await _fixture.Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName(nameof(ApprenticeController.TrainingDates));
        }
  
        internal class GetSelectDeliveryModelFixture
        {
            private readonly Mock<Interfaces.ICookieStorageService<IndexRequest>> _cookieStorageServiceMock;
            private readonly Mock<IModelMapper> _modelMapperMock;
            private readonly SelectDeliveryModelRequest _request;
            private readonly ApprenticeController _sut;
            private readonly SelectDeliveryModelViewModel _viewModel;

            public GetSelectDeliveryModelFixture()
            {
                _request = new SelectDeliveryModelRequest
                {
                    ProviderId = 2342,
                    ApprenticeshipHashedId = "KG34DF989"
                };
                _viewModel = new SelectDeliveryModelViewModel
                {
                    DeliveryModels = new List<DeliveryModel>(),
                    ProviderId = 2342,
                    CacheKey = Guid.NewGuid()
                };
                _cookieStorageServiceMock = new Mock<Interfaces.ICookieStorageService<IndexRequest>>();
                _modelMapperMock = new Mock<IModelMapper>();
                _modelMapperMock
                    .Setup(x => x.Map<SelectDeliveryModelViewModel>(_request))
                    .ReturnsAsync(_viewModel);

                _sut = new ApprenticeController(_modelMapperMock.Object, _cookieStorageServiceMock.Object,
                    Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>());
            }

            public Task<IActionResult> Act() => _sut.SelectDeliveryModel(_request);

            public GetSelectDeliveryModelFixture WithApprenticeshipStatus(ApprenticeshipStatus status)
            {
                _viewModel.ApprenticeshipStatus = status;
                return this;
            }

            public GetSelectDeliveryModelFixture WithDeliveryModel()
            {
                _viewModel.DeliveryModels.Add(new DeliveryModel());
                _viewModel.DeliveryModels.Add(new DeliveryModel());

                return this;
            }

            public void Verify_ModelMapperWasCalled(Times times) =>
                _modelMapperMock.Verify(x => x.Map<SelectDeliveryModelViewModel>(_request), times);
        }
    }
}