using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.Commitments.Shared.Models;
using SFA.DAS.Commitments.Shared.Models.ApprenticeshipCourse;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderUrlHelper;
using RedirectResult = Microsoft.AspNetCore.Mvc.RedirectResult;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    public class DraftApprenticeshipControllerTestFixture
    {
        private readonly EditDraftApprenticeshipDetails _editDraftApprenticeshipDetails;
        private readonly EditDraftApprenticeshipRequest _editDraftApprenticeshipRequest;
        private readonly NonReservationsAddDraftApprenticeshipRequest _nonReservationsAddDraftApprenticeshipRequest;
        private readonly DraftApprenticeshipController _controller;
        private readonly GetTrainingCoursesQueryResponse _courseResponse;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipRequest>> _mapper;
        private readonly Mock<IMapper<EditDraftApprenticeshipDetails, EditDraftApprenticeshipViewModel>> _editMapper;
        private readonly Mock<IMapper<EditDraftApprenticeshipViewModel, UpdateDraftApprenticeshipRequest>> _updateMapper;
        private readonly Mock<ILinkGenerator> _linkGenerator;
        private readonly Mock<ICommitmentsService> _providerCommitmentsService;
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

            _nonReservationsAddDraftApprenticeshipRequest = autoFixture.Build<NonReservationsAddDraftApprenticeshipRequest>()
                .With(x => x.ProviderId, _providerId)
                .With(x => x.CohortId, _cohortId)
                .With(x => x.CohortReference, _cohortReference)
                .Create();
            _editDraftApprenticeshipRequest = autoFixture.Build<EditDraftApprenticeshipRequest>()
                .With(x => x.CohortId, _cohortId)
                .With(x => x.DraftApprenticeshipId, _draftApprenticeshipId)
                .Create();
                ;
            _editDraftApprenticeshipDetails = autoFixture.Build<EditDraftApprenticeshipDetails>()
                .With(x => x.CohortId, _cohortId)
                .With(x => x.DraftApprenticeshipId, _draftApprenticeshipId)
                .Create();

            _createAddDraftApprenticeshipRequest = new AddDraftApprenticeshipRequest();
            _updateDraftApprenticeshipRequest = new UpdateDraftApprenticeshipRequest();

            _reservationsAddDraftApprenticeshipRequest = autoFixture.Build<ReservationsAddDraftApprenticeshipRequest>()
                .With(x => x.ProviderId, _providerId)
                .With(x => x.CohortId, _cohortId)
                .With(x => x.CohortReference, _nonReservationsAddDraftApprenticeshipRequest.CohortReference)
                .With(x => x.StartMonthYear, "012019")
                .Create();

            _courseResponse = new GetTrainingCoursesQueryResponse
            {
                TrainingCourses = new ICourse[0]
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

            _apiModelException = new CommitmentsApiModelException(new List<ErrorDetail>()
                {new ErrorDetail("Name", "Cannot be more than...")});

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetTrainingCoursesQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_courseResponse);

            _mapper = new Mock<IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipRequest>>();
            _mapper.Setup(x => x.Map(It.IsAny<AddDraftApprenticeshipViewModel>()))
                .Returns(Task.FromResult(_createAddDraftApprenticeshipRequest));

            _editMapper = new Mock<IMapper<EditDraftApprenticeshipDetails, EditDraftApprenticeshipViewModel>>();
            _editMapper.Setup(x => x.Map(It.IsAny<EditDraftApprenticeshipDetails>()))
                .Returns(Task.FromResult(_editModel));

            _updateMapper = new Mock<IMapper<EditDraftApprenticeshipViewModel, UpdateDraftApprenticeshipRequest>>();
            _updateMapper.Setup(x => x.Map(It.IsAny<EditDraftApprenticeshipViewModel>()))
                .Returns(Task.FromResult(_updateDraftApprenticeshipRequest));

            _linkGenerator = new Mock<ILinkGenerator>();
            _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                .Returns<string>(input => input);

            _providerCommitmentsService = new Mock<ICommitmentsService>();
            _providerCommitmentsService.Setup(x => x.GetCohortDetail(It.IsAny<long>()))
                .ReturnsAsync(autoFixture.Build<CohortDetails>().Create());

            _controller = new DraftApprenticeshipController(_mediator.Object, _providerCommitmentsService.Object,
                _mapper.Object, _editMapper.Object, _updateMapper.Object, _linkGenerator.Object);
        }

        public async Task<DraftApprenticeshipControllerTestFixture> AddDraftApprenticeshipWithoutReservation()
        {
            _actionResult = await _controller.AddDraftApprenticeship(_nonReservationsAddDraftApprenticeshipRequest);
            return this;
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

        public DraftApprenticeshipControllerTestFixture SetupProviderCommitmentServiceToReturnADraftApprentice()
        {
            _providerCommitmentsService
                .Setup(x => x.GetDraftApprenticeshipForCohort(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(_editDraftApprenticeshipDetails);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupAddingToThrowCommitmentsApiException()
        {
            _providerCommitmentsService
                .Setup(x => x.AddDraftApprenticeshipToCohort(It.IsAny<long>(), It.IsAny<AddDraftApprenticeshipRequest>()))
                .ThrowsAsync(_apiModelException);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupUpdatingToThrowCommitmentsApiException()
        {
            _providerCommitmentsService
                .Setup(x => x.UpdateDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<UpdateDraftApprenticeshipRequest>()))
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
            _providerCommitmentsService
                .Setup(pcs => pcs.GetCohortDetail(_cohortId))
                .ReturnsAsync(new CohortDetails {CohortId = _cohortId, IsFundedByTransfer = isFundedByTransfer });
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyAddViewHasCohortButWithoutAReservationId()
        {
            Assert.IsInstanceOf<ViewResult>(_actionResult);
            Assert.IsInstanceOf<AddDraftApprenticeshipViewModel>(((ViewResult) _actionResult).Model);

            var model = ((ViewResult) _actionResult).Model as AddDraftApprenticeshipViewModel;
            Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.ProviderId, model.ProviderId);
            Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.CohortId, model.CohortId);
            Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.CohortReference, model.CohortReference);
            Assert.IsNull(model.ReservationId);

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyAddViewHasCohortWithAReservationId()
        {
            Assert.IsInstanceOf<ViewResult>(_actionResult);
            Assert.IsInstanceOf<AddDraftApprenticeshipViewModel>(((ViewResult) _actionResult).Model);

            var model = ((ViewResult) _actionResult).Model as AddDraftApprenticeshipViewModel;
            Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.ProviderId, model.ProviderId);
            Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.CohortId, model.CohortId);
            Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.CohortReference, model.CohortReference);
            Assert.IsNotNull(model.ReservationId);
            Assert.AreEqual(_reservationsAddDraftApprenticeshipRequest.ReservationId, model.ReservationId);

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

        public DraftApprenticeshipControllerTestFixture VerifyGetDraftApprenticeshipReceivesCorrectParameters()
        {
            _providerCommitmentsService.Verify(x=>x.GetDraftApprenticeshipForCohort(_editDraftApprenticeshipRequest.CohortId.Value, _editDraftApprenticeshipRequest.DraftApprenticeshipId.Value));
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
            _providerCommitmentsService.Verify(
                x => x.GetCohortDetail(_cohortId), Times.Once);
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
            _mapper.Verify(x => x.Map(_addModel), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyUpdateMappingToApiTypeIsCalled()
        {
            _updateMapper.Verify(x => x.Map(_editModel), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyApiAddMethodIsCalled()
        {
            _providerCommitmentsService.Verify(
                x => x.AddDraftApprenticeshipToCohort(_addModel.CohortId.Value, _createAddDraftApprenticeshipRequest), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyApiUpdateMethodIsCalled()
        {
            _providerCommitmentsService.Verify(
                x => x.UpdateDraftApprenticeship(_cohortId, _draftApprenticeshipId, _updateDraftApprenticeshipRequest), Times.Once);
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
