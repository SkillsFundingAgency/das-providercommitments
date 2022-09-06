using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    public class DraftApprenticeshipControllerTestFixture
    {
        private readonly GetDraftApprenticeshipResponse _draftApprenticeshipDetails;
        private readonly DraftApprenticeshipRequest _draftApprenticeshipRequest;
        private readonly GetCohortResponse _cohortResponse;
        private readonly DraftApprenticeshipController _controller;
        private readonly GetTrainingCoursesQueryResponse _courseResponse;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly Mock<ICommitmentsApiClient> _commitmentsApiClient;
        private readonly Mock<IAuthorizationService> _providerFeatureToggle;
        private readonly AddDraftApprenticeshipViewModel _addModel;
        private readonly SelectCourseViewModel _selectCourseViewModel;
        private readonly EditDraftApprenticeshipViewModel _editModel;
        private readonly AddDraftApprenticeshipRequest _createAddDraftApprenticeshipRequest;
        private readonly UpdateDraftApprenticeshipRequest _updateDraftApprenticeshipRequest;
        private readonly ReservationsAddDraftApprenticeshipRequest _reservationsAddDraftApprenticeshipRequest;
        private IActionResult _actionResult;
        private readonly CommitmentsApiModelException _apiModelException;
        private readonly long _cohortId;
        private readonly long _draftApprenticeshipId;
        private readonly int _providerId;
        private readonly string _cohortReference;
        private readonly string _draftApprenticeshipHashedId;
        private ViewDraftApprenticeshipViewModel _viewModel;
        private readonly SelectOptionsRequest _selectOptionsRequest;
        private readonly ViewSelectOptionsViewModel _viewSelectOptionsViewModel;
        private readonly ViewSelectOptionsViewModel _selectOptionsViewModel;
        private readonly Mock<ITempDataDictionary> _tempData;
        private readonly Mock<IOuterApiService> _outerApiService;
        private ValidateUlnOverlapResult _validateUlnOverlapResult;
        private Infrastructure.OuterApi.Responses.ValidateUlnOverlapOnStartDateQueryResult _validateUlnOverlapOnStartDateResult;

        public DraftApprenticeshipControllerTestFixture()
        {
            var autoFixture = new Fixture();

            _cohortId = autoFixture.Create<long>();
            _draftApprenticeshipId = autoFixture.Create<long>();
            _providerId = autoFixture.Create<int>();
            _cohortReference = autoFixture.Create<string>();
            _draftApprenticeshipHashedId = autoFixture.Create<string>();

            _draftApprenticeshipRequest = autoFixture.Build<DraftApprenticeshipRequest>()
                .With(x => x.CohortId, _cohortId)
                .With(x => x.DraftApprenticeshipId, _draftApprenticeshipId)
                .Create();

            _selectOptionsRequest = autoFixture.Build<SelectOptionsRequest>()
                .With(c => c.CohortId, _cohortId)
                .With(x => x.DraftApprenticeshipId, _draftApprenticeshipId)
                .Create();

            _selectOptionsViewModel = autoFixture.Build<ViewSelectOptionsViewModel>()
                .With(c => c.CohortId, _cohortId)
                .With(x => x.DraftApprenticeshipId, _draftApprenticeshipId)
                .Create();

            _draftApprenticeshipDetails = autoFixture.Build<GetDraftApprenticeshipResponse>()
                .With(x => x.Id, _draftApprenticeshipId)
                .Create();

            _createAddDraftApprenticeshipRequest = new AddDraftApprenticeshipRequest();
            _updateDraftApprenticeshipRequest = new UpdateDraftApprenticeshipRequest();

            _reservationsAddDraftApprenticeshipRequest = autoFixture.Build<ReservationsAddDraftApprenticeshipRequest>()
                .With(x => x.ProviderId, _providerId)
                .With(x => x.CohortId, _cohortId)
                .With(x => x.CohortReference, _cohortReference)
                .With(x => x.StartMonthYear, "012019")
                .Create();

            _courseResponse = new GetTrainingCoursesQueryResponse
            {
                TrainingCourses = new TrainingProgramme[0]
            };

            _selectCourseViewModel = new SelectCourseViewModel()
            {
                CourseCode = "123",
                ProviderId = _providerId,
                CohortId = _cohortId,
                CohortReference = _cohortReference,
                DeliveryModel = DeliveryModel.Regular,
            };

            _addModel = new AddDraftApprenticeshipViewModel
            {
                CourseCode = "123",
                ProviderId = _providerId,
                CohortId = _cohortId,
                CohortReference = _cohortReference,
                DeliveryModel = DeliveryModel.Regular,
            };

            _editModel = new EditDraftApprenticeshipViewModel
            {
                ProviderId = _providerId,
                CohortId = _cohortId,
                CohortReference = _cohortReference,
                DraftApprenticeshipId = _draftApprenticeshipId,
                DraftApprenticeshipHashedId = _draftApprenticeshipHashedId,
                DeliveryModel = DeliveryModel.Regular,
            };

            _viewModel = new ViewDraftApprenticeshipViewModel
            {
                ProviderId = _providerId,
                CohortReference = _cohortReference
            };

            _validateUlnOverlapResult = new ValidateUlnOverlapResult
            {
                HasOverlappingEndDate = false,
                HasOverlappingStartDate = false,
                ULN = "XXXX"
            };

            _viewSelectOptionsViewModel = autoFixture.Build<ViewSelectOptionsViewModel>().Create();

            _cohortResponse = autoFixture.Build<GetCohortResponse>()
                .With(x => x.LevyStatus, ApprenticeshipEmployerType.Levy)
                .With(x => x.ChangeOfPartyRequestId, default(long?))
                .Create();

            _apiModelException = new CommitmentsApiModelException(new List<ErrorDetail>()
                {new ErrorDetail("Name", "Cannot be more than...")});

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetTrainingCoursesQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_courseResponse);

            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<AddDraftApprenticeshipRequest>(It.IsAny<AddDraftApprenticeshipViewModel>()))
                .ReturnsAsync(_createAddDraftApprenticeshipRequest);

            _modelMapper.Setup(x => x.Map<UpdateDraftApprenticeshipRequest>(It.IsAny<EditDraftApprenticeshipViewModel>()))
                .ReturnsAsync(_updateDraftApprenticeshipRequest);

            _modelMapper.Setup(x => x.Map<AddDraftApprenticeshipViewModel>(It.IsAny<ReservationsAddDraftApprenticeshipRequest>()))
                .ReturnsAsync(_addModel);

            _modelMapper.Setup(x => x.Map<UpdateDraftApprenticeshipRequest>(It.IsAny<GetDraftApprenticeshipResponse>()))
                .ReturnsAsync(_updateDraftApprenticeshipRequest);

            _modelMapper.Setup(x => x.Map<UpdateDraftApprenticeshipRequest>(It.IsAny<ViewSelectOptionsViewModel>()))
                .ReturnsAsync(_updateDraftApprenticeshipRequest);

            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_cohortResponse);
            _commitmentsApiClient.Setup(
                x => x.AddDraftApprenticeship(_addModel.CohortId.Value, _createAddDraftApprenticeshipRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AddDraftApprenticeshipResponse
                {
                    DraftApprenticeshipId = _draftApprenticeshipId
                });

            _commitmentsApiClient.Setup(x => x.ValidateUlnOverlap(It.IsAny<ValidateUlnOverlapRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => _validateUlnOverlapResult);
            _providerFeatureToggle = new Mock<IAuthorizationService>();
            _providerFeatureToggle.Setup(x => x.IsAuthorized(It.IsAny<string>())).Returns(false);
            _providerFeatureToggle.Setup(x => x.IsAuthorized(ProviderFeature.RecognitionOfPriorLearning)).Returns(true);

            _tempData = new Mock<ITempDataDictionary>();

            var encodingService = new Mock<IEncodingService>();
            encodingService.Setup(x => x.Encode(_draftApprenticeshipId, EncodingType.ApprenticeshipId))
                .Returns(_draftApprenticeshipHashedId);

            _outerApiService = new Mock<IOuterApiService>();
            _outerApiService.Setup(x => x.ValidateUlnOverlapOnStartDate(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => _validateUlnOverlapOnStartDateResult);

            _controller = new DraftApprenticeshipController(
                _mediator.Object,
                _commitmentsApiClient.Object,
                _modelMapper.Object,
                encodingService.Object,
                _providerFeatureToggle.Object, _outerApiService.Object);
            _controller.TempData = _tempData.Object;
        }

        public DraftApprenticeshipControllerTestFixture SetupStartDateOverlap(bool overlapStartDate, bool overlapEndDate)
        {
            _validateUlnOverlapOnStartDateResult = new Infrastructure.OuterApi.Responses.ValidateUlnOverlapOnStartDateQueryResult
            {
                HasOverlapWithApprenticeshipId = 1,
                HasStartDateOverlap = overlapStartDate
            };

            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupAddDraftApprenticeshipViewModelForStartDateOverlap()
        {
            _addModel.StartMonth = 1;
            _addModel.StartYear = 2022;
            _addModel.EndMonth = 1;
            _addModel.EndYear = 2023;
            _addModel.Uln = "XXXX";

            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupEditDraftApprenticeshipViewModelForStartDateOverlap()
        {
            _editModel.StartMonth = 1;
            _editModel.StartYear = 2022;
            _editModel.EndMonth = 1;
            _editModel.EndYear = 2023;
            _editModel.Uln = "XXXX";

            return this;
        }

        public async Task<DraftApprenticeshipControllerTestFixture> AddDraftApprenticeshipWithReservation()
        {
            _actionResult = await _controller.AddDraftApprenticeship(_reservationsAddDraftApprenticeshipRequest);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture AddNewDraftApprenticeshipWithReservation()
        {
            _actionResult = _controller.AddNewDraftApprenticeship(_reservationsAddDraftApprenticeshipRequest);
            return this;
        }


        public async Task<DraftApprenticeshipControllerTestFixture> EditDraftApprenticeship()
        {
            _modelMapper.Setup(x => x.Map<IDraftApprenticeshipViewModel>(_draftApprenticeshipRequest))
                .ReturnsAsync(_editModel);
            _actionResult = await _controller.ViewEditDraftApprenticeship(_draftApprenticeshipRequest);
            return this;
        }

        public async Task<DraftApprenticeshipControllerTestFixture> ViewDraftApprenticeship()
        {
            _modelMapper.Setup(x => x.Map<IDraftApprenticeshipViewModel>(_draftApprenticeshipRequest))
                .ReturnsAsync(_viewModel);
            _actionResult = await _controller.ViewEditDraftApprenticeship(_draftApprenticeshipRequest);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture ReturnNoMappedOptions()
        {
            _viewSelectOptionsViewModel.Options = new List<string>();
            return this;
        }

        public async Task<DraftApprenticeshipControllerTestFixture> ViewStandardOptions()
        {
            _modelMapper.Setup(x => x.Map<ViewSelectOptionsViewModel>(_selectOptionsRequest))
                .ReturnsAsync(_viewSelectOptionsViewModel);

            _actionResult = await _controller.SelectOptions(_selectOptionsRequest);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetUpNoStandardSelected()
        {
            _selectCourseViewModel.CourseCode = "";
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetUpFlexibleStandardSelected()
        {
            _addModel.CourseCode = "456FlexiJob";
            return this;
        }

        internal async Task<DraftApprenticeshipControllerTestFixture> PostToSelectStandard()
        {
            _actionResult = await _controller.SetCourse(_selectCourseViewModel);
            return this;
        }

        public async Task<DraftApprenticeshipControllerTestFixture> PostToAddDraftApprenticeship(string changeCourse = null, string changeDeliveryModel = null)
        {
            _actionResult = await _controller.AddDraftApprenticeship(changeCourse, changeDeliveryModel, _addModel);
            return this;
        }

        public async Task<DraftApprenticeshipControllerTestFixture> PostToEditDraftApprenticeship(string changeCourse = null, string changeDeliveryModel = null)
        {
            _actionResult = await _controller.EditDraftApprenticeship(changeCourse, changeDeliveryModel, _editModel);
            return this;
        }

        public async Task<DraftApprenticeshipControllerTestFixture> PostToSelectOption()
        {
            _actionResult = await _controller.PostSelectOptions(_selectOptionsViewModel);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupHasChosenToChooseOptionLater()
        {
            _selectOptionsViewModel.SelectedOption = "-1";
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupUpdateRequestCourseOptionChooseLater()
        {
            _updateDraftApprenticeshipRequest.CourseOption = string.Empty;
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupUpdateRequestCourseOption()
        {
            _updateDraftApprenticeshipRequest.CourseOption = _selectOptionsViewModel.SelectedOption;
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupProviderIdOnEditRequest(long providerId)
        {
            _draftApprenticeshipRequest.ProviderId = providerId;
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetApprenticeshipStarting(string startDate)
        {
            if (startDate != null)
            {
                var startDate_ = DateTime.Parse(startDate);
                _addModel.StartDate = new MonthYearModel($"{startDate_.Month}{startDate_.Year}");
                _editModel.StartDate = _addModel.StartDate;
                _viewModel.StartDate = startDate_;
            }

            SetupCommitmentsApiToReturnADraftApprentice();
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupCohortFundedByTransfer(bool transferFunded)
        {
            if (transferFunded)
            {
                _cohortResponse.TransferSenderId = 9879;
            }
            else
            {
                _cohortResponse.TransferSenderId = null;
            }
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetCohortWithChangeOfParty(bool isChangeOfParty)
        {
            if (isChangeOfParty)
            {
                _cohortResponse.ChangeOfPartyRequestId = 12345;
            }
            else
            {
                _cohortResponse.ChangeOfPartyRequestId = null;
            }
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupLevyStatus(ApprenticeshipEmployerType status)
        {
            _cohortResponse.LevyStatus = status;
            return this;
        }


        public DraftApprenticeshipControllerTestFixture SetupTempDraftApprenticeship()
        {
            object addModelAsString = JsonConvert.SerializeObject(_addModel);
            _tempData.Setup(x => x.TryGetValue(nameof(AddDraftApprenticeshipViewModel), out addModelAsString));
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupTempEditDraftApprenticeship()
        {
            object addModelAsString = JsonConvert.SerializeObject(_editModel);
            _tempData.Setup(x => x.TryGetValue(nameof(EditDraftApprenticeshipViewModel), out addModelAsString));
            return this;
        }

        public void VerifyViewModelFromTempDataHasDeliveryModelAndCourseValuesSet()
        {
            var model = _actionResult.VerifyReturnsViewModel().WithModel<AddDraftApprenticeshipViewModel>();
            ViewModelHasRequestDeliveryModelAndCourseCode(model);
        }

        public DraftApprenticeshipControllerTestFixture SetupCommitmentsApiToReturnADraftApprentice()
        {
            _commitmentsApiClient
                .Setup(x => x.GetDraftApprenticeship(_cohortId, _draftApprenticeshipId, It.IsAny<CancellationToken>())).ReturnsAsync(_draftApprenticeshipDetails);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyCommitmentsApiGetDraftApprenticeshipNotCalled()
        {
            _commitmentsApiClient
                .Verify(x => x.GetDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(),
                    It.IsAny<CancellationToken>()), Times.Never);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetUpStandardToReturnOptions()
        {
            _draftApprenticeshipDetails.HasStandardOptions = true;
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetUpStandardToReturnNoOptions()
        {
            _draftApprenticeshipDetails.HasStandardOptions = false;
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupAddingToThrowCommitmentsApiException()
        {
            _commitmentsApiClient
                .Setup(x => x.AddDraftApprenticeship(It.IsAny<long>(), It.IsAny<AddDraftApprenticeshipRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(_apiModelException);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupUpdatingToThrowCommitmentsApiException()
        {
            _commitmentsApiClient
                .Setup(x => x.UpdateDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<UpdateDraftApprenticeshipRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(_apiModelException);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupModelStateToBeInvalid()
        {
            _controller.ModelState.AddModelError("Error", "ErrorMessage");
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupCohortTransferFundedStatus(bool isFundedByTransfer)
        {
            _commitmentsApiClient
                .Setup(pcs => pcs.GetCohort(_cohortId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCohortResponse { CohortId = _cohortId, TransferSenderId = 1 });
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyEditDraftApprenticeshipViewModelIsSentToViewResult()
        {
            Assert.IsInstanceOf<ViewResult>(_actionResult);
            Assert.IsInstanceOf<EditDraftApprenticeshipViewModel>(((ViewResult)_actionResult).Model);

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyViewDraftApprenticeshipViewModelIsSentToViewResult()
        {
            Assert.IsInstanceOf<ViewResult>(_actionResult);
            Assert.IsInstanceOf<ViewDraftApprenticeshipViewModel>(((ViewResult)_actionResult).Model);

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyEditDraftApprenticeshipViewModelHasProviderIdSet()
        {
            Assert.IsInstanceOf<EditDraftApprenticeshipViewModel>(((ViewResult)_actionResult).Model);
            var model = ((ViewResult)_actionResult).Model as EditDraftApprenticeshipViewModel;
            Assert.AreEqual(_draftApprenticeshipRequest.ProviderId, model.ProviderId);

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyAddViewWasReturnedAndHasErrors()
        {
            Assert.IsInstanceOf<ViewResult>(_actionResult);
            Assert.IsInstanceOf<AddDraftApprenticeshipViewModel>(((ViewResult)_actionResult).Model);

            var view = ((ViewResult)_actionResult);
            Assert.AreEqual(1, view.ViewData.ModelState.ErrorCount);

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyEditViewWasReturnedAndHasErrors()
        {
            Assert.IsInstanceOf<ViewResult>(_actionResult);
            Assert.IsInstanceOf<EditDraftApprenticeshipViewModel>(((ViewResult)_actionResult).Model);

            var view = ((ViewResult)_actionResult);
            Assert.AreEqual(1, view.ViewData.ModelState.ErrorCount);

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyCohortDetailsWasCalledWithCorrectId()
        {
            _commitmentsApiClient.Verify(x => x.GetCohort(_cohortId, It.IsAny<CancellationToken>()), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyGetCoursesWasCalled()
        {
            _mediator.Verify(x => x.Send(It.IsAny<GetTrainingCoursesQueryRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyMappingToApiTypeIsCalled()
        {
            _modelMapper.Verify(x => x.Map<AddDraftApprenticeshipRequest>(_addModel), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyMappingFromReservationAddRequestIsCalled()
        {
            _modelMapper.Verify(x => x.Map<AddDraftApprenticeshipViewModel>(_reservationsAddDraftApprenticeshipRequest), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyUpdateMappingToApiTypeIsCalled()
        {
            _modelMapper.Verify(x => x.Map<UpdateDraftApprenticeshipRequest>(_editModel), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyApiAddMethodIsCalled()
        {
            _commitmentsApiClient.Verify(
                x => x.AddDraftApprenticeship(_addModel.CohortId.Value, _createAddDraftApprenticeshipRequest, It.IsAny<CancellationToken>()), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyApiUpdateMethodIsCalled()
        {
            _commitmentsApiClient.Verify(
                x => x.UpdateDraftApprenticeship(_cohortId, _draftApprenticeshipId, _updateDraftApprenticeshipRequest, It.IsAny<CancellationToken>()), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyApiUpdateWithStandardOptionSet(string standardOption = null)
        {
            _commitmentsApiClient.Verify(
                x => x.UpdateDraftApprenticeship(_cohortId, _draftApprenticeshipId, It.Is<UpdateDraftApprenticeshipRequest>(c => c.CourseOption.Equals(standardOption ?? _updateDraftApprenticeshipRequest.CourseOption)), It.IsAny<CancellationToken>()), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyRedirectedBackToCohortDetailsPage()
        {
            _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("Details");

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyRedirectedBackToSelectStandardPage()
        {
            _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("SelectCourse");

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyRedirectedToSelectCoursePage()
        {
            _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("SelectCourse");
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyRedirectedToSelectForEditCoursePage()
        {
            _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("SelectCourseForEdit");
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyRedirectedToSelectDeliveryModelPage()
        {
            _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("SelectDeliveryModel");
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyRedirectedToSelectDeliveryForEditModelPage()
        {
            _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("SelectDeliveryModelForEdit");
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyRedirectedToAddDraftApprenticeshipDetails()
        {
            _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("AddDraftApprenticeship");
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyRedirectToSelectOptionsPage()
        {
            _actionResult.VerifyRedirectsToSelectOptionsPage(_draftApprenticeshipHashedId);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyRedirectToRecognisePriorLearningPage()
        {
            _actionResult.VerifyRedirectsToRecognisePriorLearningPage(_draftApprenticeshipHashedId);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifySelectOptionsViewReturned()
        {
            var viewResult = _actionResult as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("SelectStandardOption", viewResult.ViewName);
            var model = viewResult.Model as ViewSelectOptionsViewModel;
            Assert.IsNotNull(model);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyWeGetABadRequestResponse()
        {
            Assert.IsInstanceOf<BadRequestObjectResult>(_actionResult);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyWhetherFrameworkCourseWereRequested(bool expectFrameworkCoursesToBeRequested)
        {
            _mediator
                .Verify(m => m.Send(
                    It.Is<GetTrainingCoursesQueryRequest>(request => request.IncludeFrameworks == expectFrameworkCoursesToBeRequested),
                    It.IsAny<CancellationToken>()),
                 Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture ViewModelHasRequestDeliveryModelAndCourseCode(AddDraftApprenticeshipViewModel model)
        {
            if (model.DeliveryModel != _reservationsAddDraftApprenticeshipRequest.DeliveryModel || model.CourseCode != _reservationsAddDraftApprenticeshipRequest.CourseCode)
            {
                Assert.Fail("DeliveryModel and CourseCode must match Request Value");
            }

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyUserRedirectedTo(string page)
        {
            _actionResult.VerifyReturnsRedirectToActionResult().WithActionName(page);
            return this;
        }
    }
}