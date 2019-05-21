using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Models;
using SFA.DAS.ProviderCommitments.Models.ApiModels;
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
        private readonly EditDraftApprenticeshipViewModel _editDraftApprenticeshipViewModel;
        private readonly EditDraftApprenticeshipDetails _editDraftApprenticeshipDetails;
        private readonly EditDraftApprenticeshipRequest _editDraftApprenticeshipRequest;
        private readonly NonReservationsAddDraftApprenticeshipRequest _nonReservationsAddDraftApprenticeshipRequest;
        private readonly DraftApprenticeshipController _controller;
        private readonly GetTrainingCoursesQueryResponse _courseResponse;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipToCohortRequest>> _mapper;
        private readonly Mock<IMapper<EditDraftApprenticeshipDetails, EditDraftApprenticeshipViewModel>> _editMapper;
        private readonly Mock<ILinkGenerator> _linkGenerator;
        private readonly Mock<IProviderCommitmentsService> _providerCommitmentsService;
        private readonly AddDraftApprenticeshipViewModel _model;
        private readonly AddDraftApprenticeshipToCohortRequest _createAddDraftApprenticeshipToCohortRequest;
        private readonly ReservationsAddDraftApprenticeshipRequest _reservationsAddDraftApprenticeshipRequest;
        private IActionResult _actionResult;
        private readonly CommitmentsApiModelException _apiModelException;

        public DraftApprenticeshipControllerTestFixture()
        {
            var autoFixture = new Fixture();

            _nonReservationsAddDraftApprenticeshipRequest = autoFixture.Build<NonReservationsAddDraftApprenticeshipRequest>().Create();
            _editDraftApprenticeshipRequest = autoFixture.Build<EditDraftApprenticeshipRequest>()
                .With(x => x.CohortId, 1).With(x => x.DraftApprenticeshipId, 2).Create();
            _editDraftApprenticeshipDetails = autoFixture.Build<EditDraftApprenticeshipDetails>().Create();
            _createAddDraftApprenticeshipToCohortRequest = new AddDraftApprenticeshipToCohortRequest();

            _reservationsAddDraftApprenticeshipRequest = autoFixture.Build<ReservationsAddDraftApprenticeshipRequest>()
                .With(x => x.CohortId, _nonReservationsAddDraftApprenticeshipRequest.CohortId)
                .With(x => x.CohortReference, _nonReservationsAddDraftApprenticeshipRequest.CohortReference)
                .With(x => x.StartMonthYear, "012019")
                .Create();

            _courseResponse = new GetTrainingCoursesQueryResponse
            {
                TrainingCourses = new ICourse[0]
            };

            _model = new AddDraftApprenticeshipViewModel
            {
                ProviderId = autoFixture.Create<int>(),
                CohortId = _nonReservationsAddDraftApprenticeshipRequest.CohortId,
                CohortReference = _nonReservationsAddDraftApprenticeshipRequest.CohortReference
            };

            _apiModelException = new CommitmentsApiModelException(new List<ErrorDetail>()
                {new ErrorDetail("Name", "Cannot be more than...")});

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetTrainingCoursesQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_courseResponse);

            _mapper = new Mock<IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipToCohortRequest>>();
            _mapper.Setup(x => x.Map(It.IsAny<AddDraftApprenticeshipViewModel>()))
                .Returns(_createAddDraftApprenticeshipToCohortRequest);

            _editMapper = new Mock<IMapper<EditDraftApprenticeshipDetails, EditDraftApprenticeshipViewModel>>();
            _editMapper.Setup(x => x.Map(It.IsAny<EditDraftApprenticeshipDetails>()))
                .Returns(_editDraftApprenticeshipViewModel);

            _linkGenerator = new Mock<ILinkGenerator>();
            _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                .Returns<string>(input => input);

            _providerCommitmentsService = new Mock<IProviderCommitmentsService>();
            _providerCommitmentsService.Setup(x => x.GetCohortDetail(It.IsAny<long>()))
                .ReturnsAsync(autoFixture.Build<CohortDetails>().Create());

            _controller = new DraftApprenticeshipController(_mediator.Object, _providerCommitmentsService.Object,
                _mapper.Object, _editMapper.Object, _linkGenerator.Object);
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

        public async Task<DraftApprenticeshipControllerTestFixture> PostDraftApprenticeship()
        {
            _actionResult = await _controller.AddDraftApprenticeship(_model);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupProviderCommitmentServiceToReturnADraftApprentice()
        {
            _providerCommitmentsService
                .Setup(x => x.GetDraftApprenticeshipForCohort(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(_editDraftApprenticeshipDetails);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupSaveToThrowCommitmentsApiException()
        {
            _providerCommitmentsService
                .Setup(x => x.AddDraftApprenticeshipToCohort(It.IsAny<AddDraftApprenticeshipToCohortRequest>()))
                .ThrowsAsync(_apiModelException);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture SetupModelStateToBeInvalid()
        {
            _controller.ModelState.AddModelError("Error", "ErrorMessage");
            return this;
        }


        public DraftApprenticeshipControllerTestFixture VerifyViewHasCohortButWithoutAReservationId()
        {
            Assert.IsInstanceOf<ViewResult>(_actionResult);
            Assert.IsInstanceOf<AddDraftApprenticeshipViewModel>(((ViewResult) _actionResult).Model);

            var model = ((ViewResult) _actionResult).Model as AddDraftApprenticeshipViewModel;
            Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.CohortId, model.CohortId);
            Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.CohortReference, model.CohortReference);
            Assert.IsNull(model.ReservationId);

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyViewHasCohortWithAReservationId()
        {
            Assert.IsInstanceOf<ViewResult>(_actionResult);
            Assert.IsInstanceOf<AddDraftApprenticeshipViewModel>(((ViewResult) _actionResult).Model);

            var model = ((ViewResult) _actionResult).Model as AddDraftApprenticeshipViewModel;
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

        public DraftApprenticeshipControllerTestFixture VerifyGetDraftApprenticeshipReceivesCorrectParameters()
        {
            _providerCommitmentsService.Verify(x=>x.GetDraftApprenticeshipForCohort(_editDraftApprenticeshipRequest.CohortId.Value, _editDraftApprenticeshipRequest.DraftApprenticeshipId.Value));
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyViewWasReturnedAndHasErrors()
        {
            Assert.IsInstanceOf<ViewResult>(_actionResult);
            Assert.IsInstanceOf<AddDraftApprenticeshipViewModel>(((ViewResult) _actionResult).Model);

            var view = ((ViewResult) _actionResult);
            Assert.AreEqual(1, view.ViewData.ModelState.ErrorCount);

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyCohortDetailsWasCalledWithCorrectId()
        {
            _providerCommitmentsService.Verify(
                x => x.GetCohortDetail(_nonReservationsAddDraftApprenticeshipRequest.CohortId.Value), Times.Once);
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
            _mapper.Verify(x => x.Map(_model), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyApiAddMethodIsCalled()
        {
            _providerCommitmentsService.Verify(
                x => x.AddDraftApprenticeshipToCohort(_createAddDraftApprenticeshipToCohortRequest), Times.Once);
            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyRedirectedBackToCohortDetailsPage()
        {
            var redirectResult = (RedirectResult) _actionResult;
            Assert.AreEqual($"{_model.ProviderId}/apprentices/{_model.CohortReference}/Details", redirectResult.Url);

            return this;
        }

        public DraftApprenticeshipControllerTestFixture VerifyWeGetABadRequestResponse()
        {
            Assert.IsInstanceOf<BadRequestObjectResult>(_actionResult);
            return this;
        }

    }
}
