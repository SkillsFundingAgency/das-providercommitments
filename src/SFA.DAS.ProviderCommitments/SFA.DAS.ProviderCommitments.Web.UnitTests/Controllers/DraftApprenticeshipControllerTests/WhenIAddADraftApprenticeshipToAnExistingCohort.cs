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
    [TestFixture]
    public class WhenIAddADraftApprenticeshipToAnExistingCohort
    {
        private AddDraftApprenticeshipToCohortTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new AddDraftApprenticeshipToCohortTestFixture();
        }

        [Test]
        public async Task IfCalledDirectlyFromProvideApprenticeshipServiceItShouldReturnAddDraftApprenticeshipViewWithCohortButWithoutAReservationId()
        {
            await _fixture.AddDraftApprenticeshipWithoutReservation();
            _fixture.VerifyViewHasCohortButWithoutAReservationId();
        }

        [Test]
        public async Task IfCalledDirectlyFromProvideApprenticeshipServiceWithAnInvalidRequestShouldGetBadResponse()
        {
            _fixture.SetupModelStateToBeInvalid();
            await _fixture.AddDraftApprenticeshipWithoutReservation();
            _fixture.VerifyWeGetABadRequestResponse();
        }

        [Test]
        public async Task IfCalledViaReservationsItShouldReturnAddDraftApprenticeshipViewWithCohortAndWithAReservationId()
        {
            await _fixture.AddDraftApprenticeshipWithReservation();
            _fixture.VerifyViewHasCohortWithAReservationId()
                .VerifyCohortDetailsWasCalledWithCorrectId()
                .VerifyGetCoursesWasCalled();
        }

        [Test]
        public async Task IfCalledViaReservationsWithAnInvalidRequestShouldGetBadResponse()
        {
            _fixture.SetupModelStateToBeInvalid();
            await _fixture.AddDraftApprenticeshipWithoutReservation();
            _fixture.VerifyWeGetABadRequestResponse();
        }


        [Test]
        public async Task AndWhenSavingTheApprenticeToCohortIsSuccessful()
        {
            await _fixture.PostDraftApprenticeship();
            _fixture.VerifyMappingToApiTypeIsCalled()
                .VerifyApiAddMethodIsCalled()
                .VerifyRedirectedBackToCohortDetailsPage();
        }

        [Test]
        public async Task AndWhenSavingFailsItShouldReturnTheViewWithModelAndErrors()
        {

            _fixture.SetupSaveToThrowCommitmentsApiException();
            await _fixture.PostDraftApprenticeship();
            _fixture.VerifyViewWasReturnedAndHasErrors()
                .VerifyCohortDetailsWasCalledWithCorrectId()
                .VerifyGetCoursesWasCalled();
        }

        [Test]
        public async Task AndWhenSavingFailsduetoModelBindingItShouldReturnTheViewWithModelAndErrors()
        {

            _fixture.SetupModelStateToBeInvalid();
            await _fixture.PostDraftApprenticeship();
            _fixture.VerifyViewWasReturnedAndHasErrors()
                .VerifyCohortDetailsWasCalledWithCorrectId()
                .VerifyGetCoursesWasCalled();
        }


        public class AddDraftApprenticeshipToCohortTestFixture
        {
            private readonly NonReservationsAddDraftApprenticeshipRequest _nonReservationsAddDraftApprenticeshipRequest;
            private readonly DraftApprenticeshipController _controller;
            private readonly GetTrainingCoursesQueryResponse _courseResponse;
            private readonly Mock<IMediator> _mediator;
            private readonly Mock<IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipToCohortRequest>> _mapper;
            private readonly Mock<ILinkGenerator> _linkGenerator;
            private readonly Mock<IProviderCommitmentsService> _providerCommitmentsService;
            private readonly AddDraftApprenticeshipViewModel _model;
            private readonly AddDraftApprenticeshipToCohortRequest _createAddDraftApprenticeshipToCohortRequest;
            private readonly ReservationsAddDraftApprenticeshipRequest _reservationsAddDraftApprenticeshipRequest;
            private IActionResult _actionResult;
            private readonly CommitmentsApiModelException _apiModelException;

            public AddDraftApprenticeshipToCohortTestFixture()
            {
                var autoFixture = new Fixture();

                _nonReservationsAddDraftApprenticeshipRequest = autoFixture.Build<NonReservationsAddDraftApprenticeshipRequest>().Create();
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

                _apiModelException = new CommitmentsApiModelException(new List<ErrorDetail>() { new ErrorDetail("Name", "Cannot be more than..." )});

                _mediator = new Mock<IMediator>();
                _mediator.Setup(x => x.Send(It.IsAny<GetTrainingCoursesQueryRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_courseResponse);

                _mapper = new Mock<IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipToCohortRequest>>();
                _mapper.Setup(x => x.Map(It.IsAny<AddDraftApprenticeshipViewModel>())).Returns(_createAddDraftApprenticeshipToCohortRequest);

                _linkGenerator = new Mock<ILinkGenerator>();
                _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                    .Returns<string>(input => input);

                _providerCommitmentsService = new Mock<IProviderCommitmentsService>();
                _providerCommitmentsService.Setup(x => x.GetCohortDetail(It.IsAny<long>()))
                    .ReturnsAsync(autoFixture.Build<CohortDetails>().Create());
                
                _controller = new DraftApprenticeshipController(_mediator.Object, _providerCommitmentsService.Object, _mapper.Object, _linkGenerator.Object);
            }

            public async Task<AddDraftApprenticeshipToCohortTestFixture> AddDraftApprenticeshipWithoutReservation()
            {
                _actionResult = await _controller.AddDraftApprenticeship(_nonReservationsAddDraftApprenticeshipRequest);
                return this;
            }

            public async Task<AddDraftApprenticeshipToCohortTestFixture> AddDraftApprenticeshipWithReservation()
            {
                _actionResult = await _controller.AddDraftApprenticeship(_reservationsAddDraftApprenticeshipRequest);
                return this;
            }

            public async Task<AddDraftApprenticeshipToCohortTestFixture> PostDraftApprenticeship()
            {
                _actionResult = await _controller.AddDraftApprenticeship(_model);
                return this;
            }

            public AddDraftApprenticeshipToCohortTestFixture SetupSaveToThrowCommitmentsApiException()
            {
                _providerCommitmentsService
                    .Setup(x => x.AddDraftApprenticeshipToCohort(It.IsAny<AddDraftApprenticeshipToCohortRequest>()))
                    .ThrowsAsync(_apiModelException);
                return this;
            }

            public AddDraftApprenticeshipToCohortTestFixture SetupModelStateToBeInvalid()
            {
                _controller.ModelState.AddModelError("Error", "ErrorMessage");
                return this;
            }


            public AddDraftApprenticeshipToCohortTestFixture VerifyViewHasCohortButWithoutAReservationId()
            {
                Assert.IsInstanceOf<ViewResult>(_actionResult);
                Assert.IsInstanceOf<AddDraftApprenticeshipViewModel>(((ViewResult)_actionResult).Model);

                var model = ((ViewResult)_actionResult).Model as AddDraftApprenticeshipViewModel;
                Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.CohortId, model.CohortId);
                Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.CohortReference, model.CohortReference);
                Assert.IsNull(model.ReservationId);

                return this;
            }

            public AddDraftApprenticeshipToCohortTestFixture VerifyViewHasCohortWithAReservationId()
            {
                Assert.IsInstanceOf<ViewResult>(_actionResult);
                Assert.IsInstanceOf<AddDraftApprenticeshipViewModel>(((ViewResult)_actionResult).Model);

                var model = ((ViewResult)_actionResult).Model as AddDraftApprenticeshipViewModel;
                Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.CohortId, model.CohortId);
                Assert.AreEqual(_nonReservationsAddDraftApprenticeshipRequest.CohortReference, model.CohortReference);
                Assert.IsNotNull(model.ReservationId);
                Assert.AreEqual(_reservationsAddDraftApprenticeshipRequest.ReservationId, model.ReservationId);

                return this;
            }

            public AddDraftApprenticeshipToCohortTestFixture VerifyViewWasReturnedAndHasErrors()
            {
                Assert.IsInstanceOf<ViewResult>(_actionResult);
                Assert.IsInstanceOf<AddDraftApprenticeshipViewModel>(((ViewResult)_actionResult).Model);

                var view = ((ViewResult) _actionResult);
                Assert.AreEqual(1, view.ViewData.ModelState.ErrorCount);

                return this;
            }

            public AddDraftApprenticeshipToCohortTestFixture VerifyCohortDetailsWasCalledWithCorrectId()
            {
                _providerCommitmentsService.Verify(x => x.GetCohortDetail(_nonReservationsAddDraftApprenticeshipRequest.CohortId.Value), Times.Once);
                return this;
            }

            public AddDraftApprenticeshipToCohortTestFixture VerifyGetCoursesWasCalled()
            {
                _mediator.Verify(x => x.Send(It.IsAny<GetTrainingCoursesQueryRequest>(), It.IsAny<CancellationToken>()), Times.Once);
                return this;
            }

            public AddDraftApprenticeshipToCohortTestFixture VerifyMappingToApiTypeIsCalled()
            {
                _mapper.Verify(x => x.Map(_model), Times.Once);
                return this;
            }

            public AddDraftApprenticeshipToCohortTestFixture VerifyApiAddMethodIsCalled()
            {
                _providerCommitmentsService.Verify(x => x.AddDraftApprenticeshipToCohort(_createAddDraftApprenticeshipToCohortRequest), Times.Once);
                return this;
            }

            public AddDraftApprenticeshipToCohortTestFixture VerifyRedirectedBackToCohortDetailsPage()
            {
                var redirectResult = (RedirectResult)_actionResult;
                Assert.AreEqual($"{_model.ProviderId}/apprentices/{_model.CohortReference}/Details", redirectResult.Url);

                return this;
            }

            public AddDraftApprenticeshipToCohortTestFixture VerifyWeGetABadRequestResponse()
            {
                Assert.IsInstanceOf<BadRequestObjectResult>(_actionResult);
                return this;
            }

        }
    }
}