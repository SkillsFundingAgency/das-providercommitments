using System;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using SFA.DAS.ProviderUrlHelper;
using CreateCohortRequest = SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort.CreateCohortRequest;
using CreateCohortResponse = SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort.CreateCohortResponse;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    public class OverlappingTrainingDateRequestControllerTestFixture
    {
        private readonly OverlappingTrainingDateRequestController _controller;
        private readonly Mock<IModelMapper> _mockModelMapper;
        private readonly DraftApprenticeshipViewModel _model;
        private IActionResult _actionResult;
        private readonly string _linkGeneratorRedirectUrl;
        private readonly Fixture _autoFixture;
        private readonly Mock<ITempDataDictionary> _tempData;
        private readonly DraftApprenticeshipOverlapOptionViewModel _draftApprenticeshipOverlapOptionViewModel;
        private readonly OverlapOptionsForChangeEmployerViewModel _overlapOptionsForChangeEmployerViewModel;
        private readonly Mock<IOuterApiService> _outerApiService;
        private readonly DraftApprenticeshipOverlapOptionRequest _draftApprenticeshipOverlapOptionRequest;
        private readonly OverlapOptionsForChangeEmployerRequest _overlapOptionsForChangeEmployerRequest;
        private GetApprenticeshipResponse _apprenticeshipDetails;
        
        private readonly DraftApprenticeshipOverlapOptionWithPendingRequest _overlapRequest;
        private readonly DraftApprenticeshipOverlapOptionWithPendingRequestViewModel _overlapViewModel;

        private readonly EmployerNotifiedRequest _employerNotifiedRequest;
        private readonly EmployerNotifiedViewModel _employerNotifiedViewModel;

        private readonly ChangeOfEmployerNotifiedRequest _changeOfEmployerNotifiedRequest;
        private readonly ChangeOfEmployerNotifiedViewModel _changeOfEmployerNotifiedViewModel;

        private readonly DraftApprenticeshipOverlapAlertRequest _draftApprenticeshipOverlapAlertRequest;
        private readonly UpdateDraftApprenticeshipApimRequest _updateDraftApprenticeshipRequest;

        public OverlappingTrainingDateRequestControllerTestFixture()
        {
            _autoFixture = new Fixture();
            _mockModelMapper = new Mock<IModelMapper>();
            
            var linkGenerator = new Mock<ILinkGenerator>();

            _draftApprenticeshipOverlapAlertRequest = _autoFixture.Create<DraftApprenticeshipOverlapAlertRequest>();
            _updateDraftApprenticeshipRequest = _autoFixture.Create<UpdateDraftApprenticeshipApimRequest>();

            _model = new DraftApprenticeshipViewModel
            {
                ProviderId = _autoFixture.Create<int>(),
                EmployerAccountLegalEntityPublicHashedId = _autoFixture.Create<string>(),
                AccountLegalEntityId = _autoFixture.Create<long>(),
                ReservationId = _autoFixture.Create<Guid>()
            };

            _draftApprenticeshipOverlapOptionRequest = new DraftApprenticeshipOverlapOptionRequest() { DraftApprenticeshipHashedId = "XXXXX", ApprenticeshipId = 1 };
            _overlapOptionsForChangeEmployerRequest = new OverlapOptionsForChangeEmployerRequest { ProviderId = 2, ApprenticeshipId = 1, CacheKey = Guid.NewGuid() };

            _tempData = new Mock<ITempDataDictionary>();

            var createCohortRequest = new CreateCohortRequest();
            _mockModelMapper
                .Setup(x => x.Map<CreateCohortRequest>(It.IsAny<AddDraftApprenticeshipViewModel>()))
                .ReturnsAsync(createCohortRequest);

            var createCohortResponse = new CreateCohortResponse
            {
                CohortId = _autoFixture.Create<long>(),
                CohortReference = _autoFixture.Create<string>(),
                HasStandardOptions = false,
                DraftApprenticeshipId = _autoFixture.Create<long>(),
            };

            _draftApprenticeshipOverlapOptionViewModel = new DraftApprenticeshipOverlapOptionViewModel
            {
                OverlapOptions = OverlapOptions.CompleteActionLater,
                ProviderId = 2,
            };

            _overlapOptionsForChangeEmployerViewModel = new OverlapOptionsForChangeEmployerViewModel
            {
                OverlapOptions = OverlapOptions.CompleteActionLater,
                ProviderId = 2,
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateCohortRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createCohortResponse);

            _linkGeneratorRedirectUrl = _autoFixture.Create<string>();
            linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                .Returns(_linkGeneratorRedirectUrl)
                .Callback((string value) => _ = value);


            _outerApiService = new Mock<IOuterApiService>();
            var commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            var validateUlnOverlapResult = new ValidateUlnOverlapResult();
            commitmentsApiClient.Setup(x => x.ValidateUlnOverlap(It.IsAny<ValidateUlnOverlapRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => validateUlnOverlapResult);

            _mockModelMapper.Setup(x => x.Map<CreateOverlappingTrainingDateApimRequest>(It.IsAny<CreateCohortResponse>())).ReturnsAsync(() => new CreateOverlappingTrainingDateApimRequest());
            _mockModelMapper.Setup(x => x.Map<CreateCohortRequest>(It.IsAny<DraftApprenticeshipOverlapOptionViewModel>())).ReturnsAsync(() => new CreateCohortRequest());
            _mockModelMapper.Setup(x => x.Map<DraftApprenticeshipOverlapOptionViewModel>(It.IsAny<DraftApprenticeshipOverlapOptionRequest>())).ReturnsAsync(() => new DraftApprenticeshipOverlapOptionViewModel());

            _apprenticeshipDetails = new GetApprenticeshipResponse()
            {
                Id = 1,
                Status = ApprenticeshipStatus.Live
            };
            commitmentsApiClient.Setup(x => x.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => _apprenticeshipDetails);

            _overlapRequest = _autoFixture.Create<DraftApprenticeshipOverlapOptionWithPendingRequest>();
            _overlapViewModel = _autoFixture.Create<DraftApprenticeshipOverlapOptionWithPendingRequestViewModel>();

            _employerNotifiedRequest = _autoFixture.Create<EmployerNotifiedRequest>();
            _employerNotifiedViewModel = _autoFixture.Create<EmployerNotifiedViewModel>();

            _changeOfEmployerNotifiedRequest = _autoFixture.Create<ChangeOfEmployerNotifiedRequest>();
            _changeOfEmployerNotifiedViewModel = _autoFixture.Create<ChangeOfEmployerNotifiedViewModel>();

            _controller = new OverlappingTrainingDateRequestController(
                mediator.Object,
                _mockModelMapper.Object,
                linkGenerator.Object,
                commitmentsApiClient.Object,
                Mock.Of<IAuthenticationService>(),
                _outerApiService.Object
                );
            _controller.TempData = _tempData.Object;
        }

        public OverlappingTrainingDateRequestControllerTestFixture SetApprenticeshipStatus(ApprenticeshipStatus status)
        {
            _apprenticeshipDetails.Status = status;
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyEnableEmployerRequestEmail(bool enabled)
        {
            enabled.Should().Be(((_actionResult as ViewResult).Model as DraftApprenticeshipOverlapOptionViewModel).EnableStopRequestEmail);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyWhenGettingOverlappingTrainingDate_ModelIsMapped()
        {
            var viewResult = _actionResult as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as DraftApprenticeshipOverlapOptionViewModel;
            model.DraftApprenticeshipHashedId.Should().Be(_draftApprenticeshipOverlapOptionRequest.DraftApprenticeshipHashedId);
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
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as DraftApprenticeshipOverlapOptionViewModel;
            model.Should().NotBeNull();
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyOverlapOptionsForChangeEmployerViewModelViewReturned()
        {
            var viewResult = _actionResult as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as OverlapOptionsForChangeEmployerViewModel;
            model.Should().NotBeNull();
            return this;
        }

        public async Task<OverlappingTrainingDateRequestControllerTestFixture> DraftApprenticeshipOverlapOptions()
        {
            _actionResult = await _controller.DraftApprenticeshipOverlapOptions(_draftApprenticeshipOverlapOptionViewModel);
            return this;
        }

        public async Task<OverlappingTrainingDateRequestControllerTestFixture> OverlapOptionsForChangeEmployer()
        {
            _actionResult = await _controller.OverlapOptionsForChangeEmployer(_overlapOptionsForChangeEmployerViewModel);
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
            _overlapOptionsForChangeEmployerViewModel.OverlapOptions = overlapOption;
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
            _mockModelMapper.Setup(m => m.Map<UpdateDraftApprenticeshipApimRequest>(It.Is<EditDraftApprenticeshipViewModel>(x => x.Uln == _model.Uln))).ReturnsAsync(_updateDraftApprenticeshipRequest);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture SetupChangeOfEmployerNotified(NextAction nextAction)
        {
            _changeOfEmployerNotifiedViewModel.NextAction = nextAction;
            _actionResult = _controller.ChangeOfEmployerNotified(_changeOfEmployerNotifiedViewModel);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture GetChangeOfEmployerNotified()
        {
            _actionResult = _controller.ChangeOfEmployerNotified(_changeOfEmployerNotifiedRequest);
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

        public OverlappingTrainingDateRequestControllerTestFixture VerifyUserRedirectedTo(string page, string controller)
        {
            _actionResult.VerifyReturnsRedirectToActionResult()
                .WithControllerName(controller)
                .WithActionName(page);
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
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as EmployerNotifiedViewModel;
            using (new AssertionScope())
            {
                model.Should().NotBeNull();
               _employerNotifiedRequest.CohortReference.Should().Be(model.CohortReference);
               _employerNotifiedRequest.ProviderId.Should().Be(model.ProviderId);
            }
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyChangeOfEmployerNotifiedViewReturned()
        {
            var viewResult = _actionResult as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ChangeOfEmployerNotifiedViewModel;
            using (new AssertionScope())
            {
                model.Should().NotBeNull();
                _changeOfEmployerNotifiedRequest.ProviderId.Should().Be(model.ProviderId);
            }
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture GetDraftApprenticeshipOverlapOptionsWithPendingRequest()
        {
            _actionResult = _controller.DraftApprenticeshipOverlapOptionsWithPendingRequest(_overlapRequest);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture GetOverlapOptionsForChangeEmployer()
        {
            _actionResult = _controller.OverlapOptionsForChangeEmployer(_overlapOptionsForChangeEmployerRequest);
            return this;
        }

        public async Task<OverlappingTrainingDateRequestControllerTestFixture> DraftApprenticeshipOverlapOptionsWithPendingRequest()
        {
            _actionResult = await _controller.DraftApprenticeshipOverlapOptionsWithPendingRequest(_overlapViewModel);
            return this;
        }

        public OverlappingTrainingDateRequestControllerTestFixture VerifyOverlapRequestsViewReturned()
        {
            var viewResult = _actionResult as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as DraftApprenticeshipOverlapOptionWithPendingRequestViewModel;
            using (new AssertionScope())
            {
                model.Should().NotBeNull();
                _overlapRequest.CohortReference.Should().Be(model.CohortReference);
                _overlapRequest.DraftApprenticeshipId.Should().Be(model.DraftApprenticeshipId);
            }
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
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as DraftApprenticeshipOverlapAlertViewModel;
            model.Should().NotBeNull();
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
            _outerApiService.Verify(m => m.UpdateDraftApprenticeship(
                _model.CohortId.Value,
                _draftApprenticeshipOverlapOptionViewModel.DraftApprenticeshipId.Value,
                It.Is<UpdateDraftApprenticeshipApimRequest>(u => u.Cost == _updateDraftApprenticeshipRequest.Cost)));

            return this;
        }
    }
}