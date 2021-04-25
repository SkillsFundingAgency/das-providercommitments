using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderUrlHelper;
using RedirectResult = Microsoft.AspNetCore.Mvc.RedirectResult;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    public class DraftApprenticeshipControllerTestFixture
    {
        private readonly GetDraftApprenticeshipResponse _editDraftApprenticeshipDetails;
        private readonly EditDraftApprenticeshipRequest _editDraftApprenticeshipRequest;
        private readonly GetCohortResponse _cohortResponse;
        private readonly DraftApprenticeshipController _controller;
        private readonly GetTrainingCoursesQueryResponse _courseResponse;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly Mock<ILinkGenerator> _linkGenerator;
        private readonly Mock<ICommitmentsApiClient> _commitmentsApiClient;
        private readonly AddDraftApprenticeshipViewModel _addModel;
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

        public DraftApprenticeshipControllerTestFixture()
        {
            var autoFixture = new Fixture();

            _cohortId = autoFixture.Create<long>();
            _draftApprenticeshipId = autoFixture.Create<long>();
            _providerId = autoFixture.Create<int>();
            _cohortReference = autoFixture.Create<string>();
            _draftApprenticeshipHashedId = autoFixture.Create<string>();

            _editDraftApprenticeshipRequest = autoFixture.Build<EditDraftApprenticeshipRequest>()
                .With(x => x.CohortId, _cohortId)
                .With(x => x.DraftApprenticeshipId, _draftApprenticeshipId)
                .Create();
                
            _editDraftApprenticeshipDetails = autoFixture.Build<GetDraftApprenticeshipResponse>()
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

            _addModel = new AddDraftApprenticeshipViewModel
            {
                ProviderId = _providerId,
                CohortId = _cohortId,
                CohortReference = _cohortReference
            };

            _editModel = new EditDraftApprenticeshipViewModel
            {
                ProviderId = _providerId,
                CohortId = _cohortId,
                CohortReference = _cohortReference,
                DraftApprenticeshipId = _draftApprenticeshipId,
                DraftApprenticeshipHashedId = _draftApprenticeshipHashedId
            };

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

            _modelMapper.Setup(x => x.Map<EditDraftApprenticeshipViewModel>(It.IsAny<EditDraftApprenticeshipRequest>()))
                .ReturnsAsync(_editModel);

            _modelMapper.Setup(x => x.Map<UpdateDraftApprenticeshipRequest>(It.IsAny<EditDraftApprenticeshipViewModel>()))
                .ReturnsAsync(_updateDraftApprenticeshipRequest);

            _modelMapper.Setup(x => x.Map<AddDraftApprenticeshipViewModel>(It.IsAny<ReservationsAddDraftApprenticeshipRequest>()))
                .ReturnsAsync(_addModel);

            _linkGenerator = new Mock<ILinkGenerator>();
            _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                .Returns<string>(input => input);

            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_cohortResponse);

            _controller = new DraftApprenticeshipController(_mediator.Object,
                _linkGenerator.Object, _commitmentsApiClient.Object, _modelMapper.Object);
        }

        public async Task<DraftApprenticeshipControllerTestFixture> AddDraftApprenticeshipWithReservation()
        {
            _actionResult = await _controller.AddDraftApprenticeship(_reservationsAddDraftApprenticeshipRequest);
            return this;
        }

        public async Task<DraftApprenticeshipControllerTestFixture> EditDraftApprenticeship()
        {
            _actionResult = await _controller.EditDraftApprenticeship(_editDraftApprenticeshipRequest);
            return this;
        }

        public async Task<DraftApprenticeshipControllerTestFixture> PostToAddDraftApprenticeship()
        {
            _actionResult = await _controller.AddDraftApprenticeship(_addModel);
            return this;
        }

        public async Task<DraftApprenticeshipControllerTestFixture> PostToEditDraftApprenticeship()
        {
            _actionResult = await _controller.EditDraftApprenticeship(_editModel);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupProviderIdOnEditRequest(long providerId)
        {
            _editDraftApprenticeshipRequest.ProviderId = providerId;
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

        public DraftApprenticeshipControllerTestFixture SetupCommitmentsApiToReturnADraftApprentice()
        {
            _commitmentsApiClient
                .Setup(x => x.GetDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(_editDraftApprenticeshipDetails);
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
                .ReturnsAsync(new GetCohortResponse {CohortId = _cohortId, TransferSenderId = 1});
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyEditDraftApprenticeshipViewModelIsSentToViewResult()
        {
            Assert.IsInstanceOf<ViewResult>(_actionResult);
            Assert.IsInstanceOf<EditDraftApprenticeshipViewModel>(((ViewResult)_actionResult).Model);

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyEditDraftApprenticeshipViewModelHasProviderIdSet()
        {
            Assert.IsInstanceOf<EditDraftApprenticeshipViewModel>(((ViewResult)_actionResult).Model);
            var model = ((ViewResult)_actionResult).Model as EditDraftApprenticeshipViewModel;
            Assert.AreEqual(_editDraftApprenticeshipRequest.ProviderId, model.ProviderId);

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyAddViewWasReturnedAndHasErrors()
        {
            Assert.IsInstanceOf<ViewResult>(_actionResult);
            Assert.IsInstanceOf<AddDraftApprenticeshipViewModel>(((ViewResult) _actionResult).Model);

            var view = ((ViewResult) _actionResult);
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

        public DraftApprenticeshipControllerTestFixture VerifyRedirectedBackToCohortDetailsPage()
        {
            var redirectResult = (RedirectResult) _actionResult;
            Assert.AreEqual($"{_providerId}/apprentices/{_cohortReference}/Details", redirectResult.Url);

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
    }
}