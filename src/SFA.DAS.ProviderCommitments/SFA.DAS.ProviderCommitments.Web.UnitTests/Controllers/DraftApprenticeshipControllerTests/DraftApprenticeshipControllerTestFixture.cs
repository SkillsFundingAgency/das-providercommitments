using System;
using System.Collections.Generic;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderUrlHelper;
using SelectCourseViewModel = SFA.DAS.ProviderCommitments.Web.Models.SelectCourseViewModel;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests;

public class DraftApprenticeshipControllerTestFixture
{
    private readonly GetDraftApprenticeshipResponse _draftApprenticeshipDetails;
    private readonly DraftApprenticeshipRequest _draftApprenticeshipRequest;
    private readonly GetCohortResponse _cohortResponse;
    private readonly DraftApprenticeshipController _controller;
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IModelMapper> _modelMapper;
    private readonly Mock<ICommitmentsApiClient> _commitmentsApiClient;
    private readonly Mock<ILinkGenerator> _linkGenerator;
    private readonly AddDraftApprenticeshipViewModel _addModel;
    private readonly SelectCourseViewModel _selectCourseViewModel;
    private readonly AddAnotherApprenticeshipRedirectModel _redirectToAddAnotherModelWithCourse;
    private readonly AddAnotherApprenticeshipRedirectModel _redirectToAddAnotherModelWithLearner;
    private readonly EditDraftApprenticeshipViewModel _editModel;
    private readonly AddDraftApprenticeshipApimRequest _createAddDraftApprenticeshipRequest;
    private readonly UpdateDraftApprenticeshipApimRequest _updateDraftApprenticeshipRequest;
    private readonly ReservationsAddDraftApprenticeshipRequest _reservationsAddDraftApprenticeshipRequest;
    private readonly GetReservationIdForAddAnotherApprenticeRequest _getReservationIdForAddAnotherApprenticeRequest;
    private readonly SelectAddAnotherDraftApprenticeshipJourneyViewModel _selectAddAnotherApprenticeshipJourneyViewModel;
    private IActionResult _actionResult;
    private readonly CommitmentsApiModelException _apiModelException;
    private readonly long _cohortId;
    private readonly long _draftApprenticeshipId;
    private readonly string _draftApprenticeshipHashedId;
    private readonly ViewDraftApprenticeshipViewModel _viewModel;
    private readonly SelectOptionsRequest _selectOptionsRequest;
    private readonly ViewSelectOptionsViewModel _viewSelectOptionsViewModel;
    private readonly ViewSelectOptionsViewModel _selectOptionsViewModel;
    private readonly Mock<ITempDataDictionary> _tempData;
    private readonly Mock<IOuterApiService> _outerApiService;
    private readonly ValidateUlnOverlapResult _validateUlnOverlapResult;
    private Infrastructure.OuterApi.Responses.ValidateUlnOverlapOnStartDateQueryResult _validateUlnOverlapOnStartDateResult;

    private Mock<IConfiguration> _configuration;
    private Mock<IConfigurationSection> _configurationSection;

    public DraftApprenticeshipControllerTestFixture()
    {
        var autoFixture = new Fixture();

        _cohortId = autoFixture.Create<long>();
        _draftApprenticeshipId = autoFixture.Create<long>();
        var providerId = autoFixture.Create<int>();
        var cohortReference = autoFixture.Create<string>();
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

        _getReservationIdForAddAnotherApprenticeRequest = autoFixture
            .Build<GetReservationIdForAddAnotherApprenticeRequest>().Without(x => x.TransferSenderHashedId)
            .Create();

        _selectAddAnotherApprenticeshipJourneyViewModel = autoFixture.Create<SelectAddAnotherDraftApprenticeshipJourneyViewModel>();

        _createAddDraftApprenticeshipRequest = new AddDraftApprenticeshipApimRequest();
        _updateDraftApprenticeshipRequest = new UpdateDraftApprenticeshipApimRequest();

        _reservationsAddDraftApprenticeshipRequest = autoFixture.Build<ReservationsAddDraftApprenticeshipRequest>()
            .With(x => x.ProviderId, providerId)
            .With(x => x.CohortId, _cohortId)
            .With(x => x.CohortReference, cohortReference)
            .With(x => x.StartMonthYear, "012019")
            .Create();

        var courseResponse = new GetTrainingCoursesQueryResponse
        {
            TrainingCourses = Array.Empty<TrainingProgramme>()
        };

        _selectCourseViewModel = new SelectCourseViewModel()
        {
            CourseCode = "123",
            ProviderId = providerId,
            CohortId = _cohortId,
            CohortReference = cohortReference,
            DeliveryModel = DeliveryModel.Regular,
        };

        _addModel = new AddDraftApprenticeshipViewModel
        {
            CourseCode = "123",
            ProviderId = providerId,
            CohortId = _cohortId,
            CohortReference = cohortReference,
            DeliveryModel = DeliveryModel.Regular,
        };

        _editModel = new EditDraftApprenticeshipViewModel
        {
            ProviderId = providerId,
            CohortId = _cohortId,
            CohortReference = cohortReference,
            DraftApprenticeshipId = _draftApprenticeshipId,
            DraftApprenticeshipHashedId = _draftApprenticeshipHashedId,
            DeliveryModel = DeliveryModel.Regular,
        };

        _viewModel = new ViewDraftApprenticeshipViewModel
        {
            ProviderId = providerId,
            CohortReference = cohortReference
        };

        _validateUlnOverlapResult = new ValidateUlnOverlapResult
        {
            HasOverlappingEndDate = false,
            HasOverlappingStartDate = false,
            ULN = "XXXX"
        };

        _redirectToAddAnotherModelWithCourse = autoFixture.Build<AddAnotherApprenticeshipRedirectModel>()
            .With(x => x.UseLearnerData, false).Create();

        _redirectToAddAnotherModelWithLearner = autoFixture.Build<AddAnotherApprenticeshipRedirectModel>()
            .With(x => x.UseLearnerData, true).Create();

        _viewSelectOptionsViewModel = autoFixture.Build<ViewSelectOptionsViewModel>().Create();

        _cohortResponse = autoFixture.Build<GetCohortResponse>()
            .With(x => x.LevyStatus, ApprenticeshipEmployerType.Levy)
            .With(x => x.ChangeOfPartyRequestId, default(long?))
            .Create();

        _apiModelException = new CommitmentsApiModelException(new List<ErrorDetail>()
            {new("Name", "Cannot be more than...")});

        _mediator = new Mock<IMediator>();
        _mediator.Setup(x => x.Send(It.IsAny<GetTrainingCoursesQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(courseResponse);

        _modelMapper = new Mock<IModelMapper>();
        _modelMapper.Setup(x => x.Map<AddDraftApprenticeshipApimRequest>(It.IsAny<AddDraftApprenticeshipViewModel>()))
            .ReturnsAsync(_createAddDraftApprenticeshipRequest);

        _modelMapper.Setup(x => x.Map<UpdateDraftApprenticeshipApimRequest>(It.IsAny<EditDraftApprenticeshipViewModel>()))
            .ReturnsAsync(_updateDraftApprenticeshipRequest);

        _modelMapper.Setup(x => x.Map<AddDraftApprenticeshipViewModel>(It.IsAny<ReservationsAddDraftApprenticeshipRequest>()))
            .ReturnsAsync(_addModel);

        _modelMapper.Setup(x => x.Map<UpdateDraftApprenticeshipApimRequest>(It.IsAny<GetDraftApprenticeshipResponse>()))
            .ReturnsAsync(_updateDraftApprenticeshipRequest);

        _modelMapper.Setup(x => x.Map<UpdateDraftApprenticeshipApimRequest>(It.IsAny<ViewSelectOptionsViewModel>()))
            .ReturnsAsync(_updateDraftApprenticeshipRequest);

        _modelMapper.Setup(x => x.Map<AddAnotherApprenticeshipRedirectModel>(It.Is<BaseReservationsAddDraftApprenticeshipRequest>(p=>p.UseLearnerData == false)))
            .ReturnsAsync(_redirectToAddAnotherModelWithCourse);

        _modelMapper.Setup(x => x.Map<AddAnotherApprenticeshipRedirectModel>(It.Is<BaseReservationsAddDraftApprenticeshipRequest>(p => p.UseLearnerData == true)))
            .ReturnsAsync(_redirectToAddAnotherModelWithLearner);

        _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
        _commitmentsApiClient.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_cohortResponse);
        _commitmentsApiClient.Setup(x => x.ValidateUlnOverlap(It.IsAny<ValidateUlnOverlapRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => _validateUlnOverlapResult);
        var providerFeatureToggle = new Mock<IAuthorizationService>();
        providerFeatureToggle.Setup(x => x.IsAuthorized(It.IsAny<string>())).Returns(false);

        _tempData = new Mock<ITempDataDictionary>();

        var encodingService = new Mock<IEncodingService>();
        encodingService.Setup(x => x.Encode(_draftApprenticeshipId, EncodingType.ApprenticeshipId))
            .Returns(_draftApprenticeshipHashedId);

        _outerApiService = new Mock<IOuterApiService>();
        _outerApiService.Setup(x => x.ValidateUlnOverlapOnStartDate(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => _validateUlnOverlapOnStartDateResult);
        _outerApiService.Setup(
                x => x.AddDraftApprenticeship(_addModel.CohortId.Value, _createAddDraftApprenticeshipRequest))
            .ReturnsAsync(new Infrastructure.OuterApi.Responses.AddDraftApprenticeshipResponse
            {
                DraftApprenticeshipId = _draftApprenticeshipId
            });

        _configurationSection = new Mock<IConfigurationSection>();
        SetupIlrConfigurationSection(false);
 
        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(c => c.GetSection("ILRFeaturesEnabled")).Returns(_configurationSection.Object);

        _controller = new DraftApprenticeshipController(
            _mediator.Object,
            _commitmentsApiClient.Object,
            _modelMapper.Object,
            encodingService.Object,
            providerFeatureToggle.Object, 
            _outerApiService.Object, 
            Mock.Of<IAuthenticationService>()
        );
            
        _controller.TempData = _tempData.Object;

        _linkGenerator = new Mock<ILinkGenerator>();
        _linkGenerator.Setup(x => x.ReservationsLink(It.IsAny<string>()))
            .Returns((string url) => "http://reservations/" + url);
    }

    public DraftApprenticeshipControllerTestFixture SetupIlrConfigurationSection(bool status)
    {
        _configurationSection.Setup(s => s.Value).Returns(status.ToString().ToLower);
        return this;
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

    public async Task<DraftApprenticeshipControllerTestFixture> AddNewDraftApprenticeshipWithReservation()
    {
        _actionResult = await _controller.AddNewDraftApprenticeship(_reservationsAddDraftApprenticeshipRequest, _configuration.Object);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture SetUseLearnerData(bool useLearnerData)
    {
        _getReservationIdForAddAnotherApprenticeRequest.UseLearnerData = useLearnerData;
        return this;
    }

    public DraftApprenticeshipControllerTestFixture GetReservationId(string transferSenderId = null)
    {
        if (transferSenderId != null)
        {
            _getReservationIdForAddAnotherApprenticeRequest.TransferSenderHashedId = transferSenderId;
        }
        _actionResult = _controller.GetReservationId(_getReservationIdForAddAnotherApprenticeRequest, _linkGenerator.Object);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture GotoSelectHowPage()
    {
        _actionResult = _controller.AddAnotherSelectMethod(_getReservationIdForAddAnotherApprenticeRequest);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture PostToAddAnotherSelectionMethod(AddAnotherDraftApprenticeshipJourneyOptions? option)
    {
        _selectAddAnotherApprenticeshipJourneyViewModel.Selection = option;
        _actionResult = _controller.AddAnotherSelectMethod(_selectAddAnotherApprenticeshipJourneyViewModel);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture PostToAddAnotherSelectionMethod()
    {
        _actionResult = _controller.AddAnotherSelectMethod(_selectAddAnotherApprenticeshipJourneyViewModel);
        return this;
    }

    public async Task<DraftApprenticeshipControllerTestFixture> EditDraftApprenticeship()
    {
        _modelMapper.Setup(x => x.Map<IDraftApprenticeshipViewModel>(_draftApprenticeshipRequest))
            .ReturnsAsync(_editModel);
        _actionResult = await _controller.EditDraftApprenticeship(_draftApprenticeshipRequest);
        return this;
    }

    public async Task<DraftApprenticeshipControllerTestFixture> ViewDraftApprenticeship()
    {
        _modelMapper.Setup(x => x.Map<IDraftApprenticeshipViewModel>(_draftApprenticeshipRequest))
            .ReturnsAsync(_viewModel);
        _actionResult = await _controller.EditDraftApprenticeship(_draftApprenticeshipRequest);
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
        
    public DraftApprenticeshipControllerTestFixture SetupUpdateRequestCourseOption()
    {
        _updateDraftApprenticeshipRequest.CourseOption = _selectOptionsViewModel.SelectedOption;
        return this;
    }

    public DraftApprenticeshipControllerTestFixture SetApprenticeshipStarting(string startDateAsString, bool actual = false)
    {
        if (startDateAsString != null)
        {
            var startDate = DateTime.Parse(startDateAsString);
            if (actual)
            {
                _addModel.StartDate = new MonthYearModel("");
                _addModel.ActualStartDate = new DateModel(startDate);
                _editModel.StartDate = _addModel.StartDate;
                _editModel.ActualStartDate = _addModel.ActualStartDate;
                _viewModel.StartDate = null;
                _viewModel.ActualStartDate = startDate;
            }
            else
            {
                _addModel.StartDate = new MonthYearModel($"{startDate.Month}{startDate.Year}");
                _addModel.ActualStartDate = new DateModel();
                _editModel.StartDate = _addModel.StartDate;
                _editModel.ActualStartDate = _addModel.ActualStartDate;
                _viewModel.StartDate = startDate;
                _viewModel.ActualStartDate = null;
            }
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

    public DraftApprenticeshipControllerTestFixture SetupUseLearnerData(bool useLearnerData)
    {
        _getReservationIdForAddAnotherApprenticeRequest.UseLearnerData = useLearnerData;
        _reservationsAddDraftApprenticeshipRequest.UseLearnerData = useLearnerData;
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
        _outerApiService
            .Setup(x => x.AddDraftApprenticeship(It.IsAny<long>(), It.IsAny<AddDraftApprenticeshipApimRequest>()))
            .ThrowsAsync(_apiModelException);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture SetupUpdatingToThrowCommitmentsApiException()
    {
        _outerApiService
            .Setup(x => x.UpdateDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<UpdateDraftApprenticeshipApimRequest>()))
            .ThrowsAsync(_apiModelException);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyEditDraftApprenticeshipViewModelIsSentToViewResult()
    {
        using (new AssertionScope())
        {
            _actionResult.Should().BeAssignableTo<ViewResult>();
            ((ViewResult)_actionResult).Model.Should().BeAssignableTo<EditDraftApprenticeshipViewModel>();
        }

        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyViewDraftApprenticeshipViewModelIsSentToViewResult()
    {
        using (new AssertionScope())
        {
            _actionResult.Should().BeAssignableTo<ViewResult>();
            ((ViewResult)_actionResult).Model.Should().BeAssignableTo<ViewDraftApprenticeshipViewModel>();
        }

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
        _modelMapper.Verify(x => x.Map<AddDraftApprenticeshipApimRequest>(_addModel), Times.Once);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyMappingFromReservationAddRequestIsCalled()
    {
        _modelMapper.Verify(x => x.Map<AddDraftApprenticeshipViewModel>(_reservationsAddDraftApprenticeshipRequest), Times.Once);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyUpdateMappingToApiTypeIsCalled()
    {
        _modelMapper.Verify(x => x.Map<UpdateDraftApprenticeshipApimRequest>(_editModel), Times.Once);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyApiAddMethodIsCalled()
    {
        _outerApiService.Verify(
            x => x.AddDraftApprenticeship(_addModel.CohortId.Value, _createAddDraftApprenticeshipRequest), Times.Once);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyApiUpdateMethodIsCalled()
    {
        _outerApiService.Verify(
            x => x.UpdateDraftApprenticeship(_cohortId, _draftApprenticeshipId, _updateDraftApprenticeshipRequest), Times.Once);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyApiUpdateWithStandardOptionSet(string standardOption = null)
    {
        _outerApiService.Verify(
            x => x.UpdateDraftApprenticeship(_cohortId, _draftApprenticeshipId, It.Is<UpdateDraftApprenticeshipApimRequest>(c => c.CourseOption.Equals(standardOption ?? _updateDraftApprenticeshipRequest.CourseOption))), Times.Once);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRedirectedBackToCohortDetailsPage()
    {
        _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("Details");
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRedirectedToRplQuestion()
    {
        _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("RecognisePriorLearning");
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRedirectedToSelectCoursePage()
    {
        _actionResult.VerifyReturnsRedirectToRouteResult().RouteName.Should().Be(RouteNames.SelectCourse);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRedirectedToSelectLearnerPage()
    {
        _actionResult.VerifyReturnsRedirectToRouteResult().RouteName.Should().Be(RouteNames.SelectLearnerRecord);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRedirectedToReservationsPage()
    {
        var redirect = _actionResult.VerifyReturnsRedirect();
        redirect.Url.Should().Contain($"/{_getReservationIdForAddAnotherApprenticeRequest.ProviderId}/reservations/{_getReservationIdForAddAnotherApprenticeRequest.AccountLegalEntityHashedId}/select?");
        redirect.Url.Should().Contain($"cohortReference={_getReservationIdForAddAnotherApprenticeRequest.CohortReference}");
        redirect.Url.Should().Contain($"encodedPledgeApplicationId={_getReservationIdForAddAnotherApprenticeRequest.EncodedPledgeApplicationId}");
        redirect.Url.Should().Contain($"encodedPledgeApplicationId={_getReservationIdForAddAnotherApprenticeRequest.EncodedPledgeApplicationId}");
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRedirectedToReservationsPageContainsTransferSenderId()
    {
        var redirect = _actionResult.VerifyReturnsRedirect();
        redirect.Url.Should().Contain($"transferSenderId={_getReservationIdForAddAnotherApprenticeRequest.TransferSenderHashedId}");
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRedirectedToSelectForEditCoursePage()
    {
        _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("EditDraftApprenticeshipCourse");
        return this;
    }
    public DraftApprenticeshipControllerTestFixture VerifyReturnsSelectHowViewModelWithCorrectValues()
    {
        var model = _actionResult.VerifyReturnsViewModel().WithModel< SelectAddAnotherDraftApprenticeshipJourneyViewModel>();
        model.ProviderId.Should().Be(_getReservationIdForAddAnotherApprenticeRequest.ProviderId);
        model.CohortReference.Should().Be(_getReservationIdForAddAnotherApprenticeRequest.CohortReference);
        model.AccountLegalEntityHashedId.Should().Be(_getReservationIdForAddAnotherApprenticeRequest.AccountLegalEntityHashedId);
        model.UseLearnerData.Should().Be(true);

        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRedirectedToGetReservationIdEndpoint()
    {
        _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("GetReservationId");
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRouteValueContainsUseLearnerDataAs(bool useLearnerData)
    {
        var redirectResult = _actionResult as RedirectToActionResult;
        redirectResult.RouteValues.Should().Contain("useLearnerData", useLearnerData);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRouteValueContainsTransferSenderId(string transferSenderId)
    {
        var redirectResult = _actionResult as RedirectToActionResult;
        redirectResult.RouteValues.Should().Contain("transferSenderHashedId", transferSenderId);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRouteValueContainsEncodedPledgeApplicationId(string encodedPledgeApplicationId)
    {
        var redirectResult = _actionResult as RedirectToActionResult;
        redirectResult.RouteValues.Should().Contain("encodedPledgeApplicationId", encodedPledgeApplicationId);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture SetupSelectHowViewModelWithTransferSenderId(string transferSenderId)
    {
        _selectAddAnotherApprenticeshipJourneyViewModel.TransferSenderHashedId = transferSenderId;
        _selectAddAnotherApprenticeshipJourneyViewModel.EncodedPledgeApplicationId = "TEST_PLEDGE_ID";
        return this;
    }

    public DraftApprenticeshipControllerTestFixture SetupSelectHowViewModelWithEncodedPledgeApplicationId(string encodedPledgeApplicationId)
    {
        _selectAddAnotherApprenticeshipJourneyViewModel.EncodedPledgeApplicationId = encodedPledgeApplicationId;
        _selectAddAnotherApprenticeshipJourneyViewModel.TransferSenderHashedId = "TEST_TRANSFER_ID";
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
        viewResult.Should().NotBeNull();
        viewResult.ViewName.Should().Be("SelectStandardOption");
        var model = viewResult.Model as ViewSelectOptionsViewModel;
        model.Should().NotBeNull();
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

    private DraftApprenticeshipControllerTestFixture ViewModelHasRequestDeliveryModelAndCourseCode(AddDraftApprenticeshipViewModel model)
    {
        if (model.DeliveryModel != _reservationsAddDraftApprenticeshipRequest.DeliveryModel || model.CourseCode != _reservationsAddDraftApprenticeshipRequest.CourseCode)
        {
            throw new Exception("DeliveryModel and CourseCode must match Request Value");
        }

        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyUserRedirectedTo(string page)
    {
        _actionResult.VerifyReturnsRedirectToActionResult().WithActionName(page);
        return this;
    }

    public async Task<DraftApprenticeshipControllerTestFixture> SyncLearnerData()
    {
        var model = new EditDraftApprenticeshipViewModel
        {
            ProviderId = _draftApprenticeshipRequest.ProviderId,
            CohortId = _draftApprenticeshipRequest.CohortId,
            DraftApprenticeshipId = _draftApprenticeshipRequest.DraftApprenticeshipId,
            DraftApprenticeshipHashedId = _draftApprenticeshipRequest.DraftApprenticeshipHashedId,
            CohortReference = _draftApprenticeshipRequest.CohortReference
        };
        
        _actionResult = await _controller.EditDraftApprenticeship(null, null, model, "SyncLearnerData");
        return this;
    }

    public DraftApprenticeshipControllerTestFixture SetupOuterApiServiceToReturnSyncResponse(SyncLearnerDataResponse response)
    {
        var learnerDataSyncResult = new LearnerDataSyncResult
        {
            Success = response.Success,
            Message = response.Success ? "Learner data has been successfully updated." : (response.Message ?? "Failed to sync learner data."),
            CacheKey = response.Success ? Guid.NewGuid().ToString() : null
        };
        
        _modelMapper
            .Setup(x => x.Map<LearnerDataSyncResult>(It.IsAny<EditDraftApprenticeshipViewModel>()))
            .ReturnsAsync(learnerDataSyncResult);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture SetupOuterApiServiceToThrowException()
    {
        var learnerDataSyncResult = new LearnerDataSyncResult
        {
            Success = false,
            Message = "An error occurred while syncing learner data."
        };
        
        _modelMapper
            .Setup(x => x.Map<LearnerDataSyncResult>(It.IsAny<EditDraftApprenticeshipViewModel>()))
            .ReturnsAsync(learnerDataSyncResult);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyOuterApiServiceSyncLearnerDataCalled()
    {
        _modelMapper.Verify(
            x => x.Map<LearnerDataSyncResult>(It.IsAny<EditDraftApprenticeshipViewModel>()), Times.Once);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyTempDataContains(string key, string expectedValue)
    {
        _tempData.VerifySet(x => x[key] = expectedValue, Times.Once);
        return this;
    }

    public DraftApprenticeshipControllerTestFixture VerifyRedirectedToEditDraftApprenticeshipPage()
    {
        _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("EditDraftApprenticeship");
        return this;
    }


    public DraftApprenticeshipControllerTestFixture VerifyRedirectContainsLearnerDataSyncKey()
    {
        var redirectResult = _actionResult as RedirectToActionResult;
        redirectResult.RouteValues.Should().ContainKey("LearnerDataSyncKey");
        redirectResult.RouteValues["LearnerDataSyncKey"].Should().NotBeNull();
        return this;
    }


    public async Task<DraftApprenticeshipControllerTestFixture> EditDraftApprenticeshipWithLearnerDataSyncKey(string learnerDataSyncKey)
    {
        var request = new DraftApprenticeshipRequest
        {
            ProviderId = _draftApprenticeshipRequest.ProviderId,
            CohortId = _draftApprenticeshipRequest.CohortId,
            DraftApprenticeshipId = _draftApprenticeshipRequest.DraftApprenticeshipId,
            DraftApprenticeshipHashedId = _draftApprenticeshipRequest.DraftApprenticeshipHashedId,
            CohortReference = _draftApprenticeshipRequest.CohortReference
        };
        
        _modelMapper.Setup(x => x.Map<IDraftApprenticeshipViewModel>(request))
            .ReturnsAsync(_editModel);
        _actionResult = await _controller.EditDraftApprenticeship(request, learnerDataSyncKey);
        return this;
    }

}