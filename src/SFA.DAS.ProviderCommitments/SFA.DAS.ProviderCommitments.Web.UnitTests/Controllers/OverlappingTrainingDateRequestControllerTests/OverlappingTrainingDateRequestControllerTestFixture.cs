using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis.Operations;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using SFA.DAS.ProviderUrlHelper;
using CreateCohortRequest = SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort.CreateCohortRequest;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    public class OverlappingTrainingDateRequestControllerTestFixture
    {
        private readonly OverlappingTrainingDateRequestController _controller;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IModelMapper> _mockModelMapper;
        private readonly Mock<ILinkGenerator> _linkGenerator;
        private readonly DraftApprenticeshipViewModel _model;
        private readonly CreateCohortRequest _createCohortRequest;
        private readonly CreateCohortResponse _createCohortResponse;
        private IActionResult _actionResult;
        private readonly string _linkGeneratorRedirectUrl;
        private string _linkGeneratorParameter;
        private Fixture _autoFixture;
        private readonly Mock<IEncodingService> _encodingService;
        private readonly Mock<ITempDataDictionary> _tempData;
        private readonly string _draftApprenticeshipHashedId;
        private readonly DraftApprenticeshipOverlapOptionViewModel _draftApprenticeshipOverlapOptionViewModel;
        private readonly Mock<IOuterApiService> _outerApiService;
        private readonly Mock<ICommitmentsApiClient> _commitmentsApiClient;
        private readonly Mock<IFeatureTogglesService<ProviderFeatureToggle>> _featureToggleService;
        private readonly DraftApprenticeshipOverlapOptionRequest _draftApprenticeshipOverlapOptionRequest;
        private ProviderFeatureToggle _overlappingTrainingDateRequestFeatureToggle;
        private CommitmentsV2.Api.Types.Responses.GetApprenticeshipResponse _apprenticeshipDetails;
        private CommitmentsV2.Api.Types.Responses.ValidateUlnOverlapResult _validateUlnOverlapResult;
        private readonly EmployerNotifiedRequest _employerNotifiedRequest;
        private readonly EmployerNotifiedViewModel _employerNotifiedViewModel;
        private readonly DraftApprenticeshipOverlapAlertRequest _draftApprenticeshipOverlapAlertRequest;
        private readonly UpdateDraftApprenticeshipRequest _updateDraftApprenticeshipRequest;

        public OverlappingTrainingDateRequestControllerTestFixture()
        {
            _autoFixture = new Fixture();

            _draftApprenticeshipHashedId = _autoFixture.Create<string>();
            _mediator = new Mock<IMediator>();
            _mockModelMapper = new Mock<IModelMapper>();
            _linkGenerator = new Mock<ILinkGenerator>();
            _encodingService = new Mock<IEncodingService>();

            _draftApprenticeshipOverlapAlertRequest = _autoFixture.Create<DraftApprenticeshipOverlapAlertRequest>();
            _updateDraftApprenticeshipRequest = _autoFixture.Create<UpdateDraftApprenticeshipRequest>();

            _model = new DraftApprenticeshipViewModel
            {
                ProviderId = _autoFixture.Create<int>(),
                EmployerAccountLegalEntityPublicHashedId = _autoFixture.Create<string>(),
                AccountLegalEntityId = _autoFixture.Create<long>(),
                ReservationId = _autoFixture.Create<Guid>()
            };

            _draftApprenticeshipOverlapOptionRequest = new DraftApprenticeshipOverlapOptionRequest() { DraftApprenticeshipHashedId = "XXXXX", ApprenticeshipId = 1 };

            _tempData = new Mock<ITempDataDictionary>();

            _createCohortRequest = new CreateCohortRequest();
            _mockModelMapper
                .Setup(x => x.Map<CreateCohortRequest>(It.IsAny<AddDraftApprenticeshipViewModel>()))
                .ReturnsAsync(_createCohortRequest);

            _createCohortResponse = new CreateCohortResponse
            {
                CohortId = _autoFixture.Create<long>(),
                CohortReference = _autoFixture.Create<string>(),
                HasStandardOptions = false,
                DraftApprenticeshipId = _autoFixture.Create<long>(),
            };

            _draftApprenticeshipOverlapOptionViewModel = new DraftApprenticeshipOverlapOptionViewModel
            {
                OverlapOptions = OverlapOptions.AddApprenticeshipLater,
                ProviderId = 2,
            };

            _mediator.Setup(x => x.Send(It.IsAny<CreateCohortRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_createCohortResponse);

            _linkGeneratorRedirectUrl = _autoFixture.Create<string>();
            _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                .Returns(_linkGeneratorRedirectUrl)
                .Callback((string value) => _linkGeneratorParameter = value);

            var authorizationService = Mock.Of<IAuthorizationService>();
            Mock.Get(authorizationService).Setup(x =>
                x.IsAuthorized(ProviderFeature.RecognitionOfPriorLearning)).Returns(true);

            _outerApiService = new Mock<IOuterApiService>();
            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient.Setup(x => x.ValidateUlnOverlap(It.IsAny<CommitmentsV2.Api.Types.Requests.ValidateUlnOverlapRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => _validateUlnOverlapResult);

            _mockModelMapper.Setup(x => x.Map<CreateOverlappingTrainingDateApimRequest>(It.IsAny<CreateCohortResponse>())).ReturnsAsync(() => new CreateOverlappingTrainingDateApimRequest());
            _mockModelMapper.Setup(x => x.Map<CreateCohortRequest>(It.IsAny<DraftApprenticeshipOverlapOptionViewModel>())).ReturnsAsync(() => new CreateCohortRequest());
            _mockModelMapper.Setup(x => x.Map<DraftApprenticeshipOverlapOptionViewModel>(It.IsAny<DraftApprenticeshipOverlapOptionRequest>())).ReturnsAsync(() => new DraftApprenticeshipOverlapOptionViewModel());

            _overlappingTrainingDateRequestFeatureToggle = new ProviderFeatureToggle() { IsEnabled = true };
            _featureToggleService = new Mock<IFeatureTogglesService<ProviderFeatureToggle>>();
            _featureToggleService.Setup(x => x.GetFeatureToggle(ProviderFeature.OverlappingTrainingDateWithoutPrefix)).Returns(() => _overlappingTrainingDateRequestFeatureToggle);

            _apprenticeshipDetails = new CommitmentsV2.Api.Types.Responses.GetApprenticeshipResponse()
            {
                Id = 1,
                Status = CommitmentsV2.Types.ApprenticeshipStatus.Live
            };
            _commitmentsApiClient.Setup(x => x.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => _apprenticeshipDetails);

            _employerNotifiedRequest = _autoFixture.Create<EmployerNotifiedRequest>();
            _employerNotifiedViewModel = _autoFixture.Create<EmployerNotifiedViewModel>();

            _controller = new OverlappingTrainingDateRequestController(
                _mediator.Object,
                _mockModelMapper.Object,
                _linkGenerator.Object,
                _commitmentsApiClient.Object,
                authorizationService,
                _outerApiService.Object,
                _featureToggleService.Object
                );
            _controller.TempData = _tempData.Object;
        }

        public OverlappingTrainingDateRequestControllerTestFixture SetOverlappingTrainingDateRequestFeatureToggle(bool isEnabled)
        {
            _overlappingTrainingDateRequestFeatureToggle.IsEnabled = isEnabled;
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyfeatureTogglesServiceToGetOverlappingTrainingDateIsCalled()
        {
            _featureToggleService.Verify(x => x.GetFeatureToggle(ProviderFeature.OverlappingTrainingDateWithoutPrefix), Times.Once);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyModelMapperDraftApprenticeshipOverlapOptionRequestToViewModelIsCalled()
        {
            _mockModelMapper.Verify(x => x.Map<DraftApprenticeshipOverlapOptionViewModel>(It.IsAny<DraftApprenticeshipOverlapOptionRequest>()), Times.Once);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyDraftApprenticeshipOverlapOptionsViewReturned()
        {
            var viewResult = _actionResult as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as DraftApprenticeshipOverlapOptionViewModel;
            Assert.IsNotNull(model);
            return this;
        }

        public async Task<OverlappingTrainingDateRequestControllerTestFixture> DraftApprenticeshipOverlapOptions()
        {
            _actionResult = await _controller.DraftApprenticeshipOverlapOptions(_draftApprenticeshipOverlapOptionViewModel);
            return this;
        }

        public async Task<OverlappingTrainingDateRequestControllerTestFixture> GetDraftApprenticeshipOverlapOptions()
        {
            _actionResult = await _controller.DraftApprenticeshipOverlapOptions(_draftApprenticeshipOverlapOptionRequest);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture SetupStartDraftOverlapOptions(OverlapOptions overlapOption)
        {
            _draftApprenticeshipOverlapOptionViewModel.OverlapOptions = overlapOption;
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture WithCohortReference()
        {
            _draftApprenticeshipOverlapOptionViewModel.CohortReference = _autoFixture.Create<string>();
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture WithDraftApprenticeshipHashedId()
        {
            _draftApprenticeshipOverlapOptionViewModel.DraftApprenticeshipId = _autoFixture.Create<long>();
            _draftApprenticeshipOverlapOptionViewModel.DraftApprenticeshipHashedId = _autoFixture.Create<string>();
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture SetupUpdateDraftApprenticeshipRequestMapper()
        {
            _mockModelMapper.Setup(m => m.Map<UpdateDraftApprenticeshipRequest>(It.Is<EditDraftApprenticeshipViewModel>(x => x.Uln == _model.Uln))).ReturnsAsync(_updateDraftApprenticeshipRequest);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture GetEmployerNotified()
        {
            _actionResult = _controller.EmployerNotified(_employerNotifiedRequest);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture SetupEmployerNotified(NextAction nextAction)
        {
            _employerNotifiedViewModel.NextAction = nextAction;
            _actionResult = _controller.EmployerNotified(_employerNotifiedViewModel);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyUserRedirectedTo(string page)
        {
            _actionResult.VerifyReturnsRedirectToActionResult().WithActionName(page);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyUserRedirectedToUrl()
        {
            _actionResult.VerifyReturnsRedirect().WithUrl(_linkGeneratorRedirectUrl);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyOverlappingTrainingDateRequestEmailSent()
        {
            _outerApiService.Verify(x => x.CreateOverlappingTrainingDateRequest(It.IsAny<CreateOverlappingTrainingDateApimRequest>()), Times.Once);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyOverlappingTrainingDateRequestEmail_IsNotSent()
        {
            _outerApiService.Verify(x => x.CreateOverlappingTrainingDateRequest(It.IsAny<CreateOverlappingTrainingDateApimRequest>()), Times.Never);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyEmployerNotifiedViewReturned()
        {
            var viewResult = _actionResult as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as EmployerNotifiedViewModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(model.CohortReference, _employerNotifiedRequest.CohortReference);
            Assert.AreEqual(model.ProviderId, _employerNotifiedRequest.ProviderId);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture GetDraftApprenticeshipOverlapAlert()
        {
            _actionResult = _controller.DraftApprenticeshipOverlapAlert(_draftApprenticeshipOverlapAlertRequest);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyDraftApprenticeshipOverlapAlertViewReturned()
        {
            var viewResult = _actionResult as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as DraftApprenticeshipOverlapAlertViewModel;
            Assert.IsNotNull(model);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture SetupPeekTempDraftApprenticeship()
        {
            object addModelAsString = JsonConvert.SerializeObject(_model);
            _tempData.Setup(x => x.Peek(nameof(AddDraftApprenticeshipViewModel))).Returns(addModelAsString);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture SetupPeekTempEditDraftApprenticeship()
        {
            object addModelAsString = JsonConvert.SerializeObject(_model);
            _tempData.Setup(x => x.Peek(nameof(EditDraftApprenticeshipViewModel))).Returns(addModelAsString);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture SetupGetTempEditDraftApprenticeship()
        {
            _model.CohortId = _autoFixture.Create<long>();
            _model.CohortReference = _autoFixture.Create<string>();

            object addModelAsString = JsonConvert.SerializeObject(_model);
            _tempData.Setup(x => x.TryGetValue(nameof(EditDraftApprenticeshipViewModel), out addModelAsString)).Returns(true);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyPeekStoredEditDraftApprenticeshipStateIsCalled()
        {
            _tempData.Verify(mock => mock.Peek(nameof(EditDraftApprenticeshipViewModel)), Times.Once);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyExistingDraftApprenticeshipUpdated()
        {
            _commitmentsApiClient.Verify(m => m.UpdateDraftApprenticeship(
                _model.CohortId.Value,
                _draftApprenticeshipOverlapOptionViewModel.DraftApprenticeshipId.Value,
                It.Is<UpdateDraftApprenticeshipRequest>(u => u.Cost == _updateDraftApprenticeshipRequest.Cost),
                It.IsAny<CancellationToken>()));

            return this;
        }
    }
}