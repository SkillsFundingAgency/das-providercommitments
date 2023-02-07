﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenIPostDraftApprenticeshipViewModel
    {
        private UnapprovedControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new UnapprovedControllerTestFixture();
        }

        [Test]
        public async Task ThenACohortIsCreated()
        {
            await _fixture.PostDraftApprenticeshipViewModel();
            _fixture.VerifyCohortCreated();
        }

        [Test]
        public async Task ThenTheUserIsRedirectedToTheViewCohortPage()
        {
            await _fixture.PostDraftApprenticeshipViewModel();
            _fixture.VerifyUserRedirectedTo("Details");
        }

        [Test]
        public async Task ThenTheUserIsRedirectedToStandardOptionsIfAvailable()
        {
            _fixture.SetupHasOptions();
            await _fixture.PostDraftApprenticeshipViewModel();
            _fixture.VerifyUserRedirectSelectOption();
        }

        [Test]
        public async Task AndSelectCourseIsToBeChangedThenTheUserIsRedirectedToSelectCoursePage()
        {
            await _fixture.PostDraftApprenticeshipViewModel(changeCourse:"Edit");
            _fixture.VerifyUserRedirectedTo("SelectCourse");
        }

        [Test]
        public async Task AndSelectDeliveryModelIsToBeChangedThenTheUserIsRedirectedToSelectDeliveryModelPage()
        {
            await _fixture.PostDraftApprenticeshipViewModel(changeDeliveryModel: "Edit");
            _fixture.VerifyUserRedirectedTo("SelectDeliveryModel");
        }

        [Test]
        public async Task AndPilotStatusIsToBeChangedThenTheUserIsRedirectedToSelectPilotStatusPage()
        {
            await _fixture.PostDraftApprenticeshipViewModel(changePilotStatus: "Edit");
            _fixture.VerifyUserRedirectedTo("ChoosePilotStatusForDraftChange");
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task UserIsRedirectedToRecognisePriorLearningPageWhenStartDateIsAfterActivationDate(bool setActualDate)
        {
            _fixture.SetModelStartDate("012023", setActualDate);
            await _fixture.PostDraftApprenticeshipViewModel();
            _fixture.VerifyUserRedirectedTo("RecognisePriorLearning");
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public async Task UserIsNotRedirectedToRecognisePriorLearningPageWhenStartDateIsBeforeActivationDate(bool setActualDate)
        {
            _fixture.SetModelStartDate("012022", setActualDate);
            await _fixture.PostDraftApprenticeshipViewModel();
            _fixture.VerifyUserRedirectIsNotToRecognisePriorLearning();
        }

        [Test]
        public async Task AndWhenThereIsStartDateOverlap()
        {
            await _fixture.SetupStartDateOverlap(true, false).SetupAddDraftApprenticeshipViewModelForStartDateOverlap().PostDraftApprenticeshipViewModel();
            _fixture.VerifyUserRedirectedTo("DraftApprenticeshipOverlapAlert");
        }

        private class UnapprovedControllerTestFixture
        {
            private readonly CohortController _controller;
            private readonly Mock<IMediator> _mediator;
            private readonly Mock<IModelMapper> _mockModelMapper;
            private readonly Mock<ILinkGenerator> _linkGenerator;
            private readonly AddDraftApprenticeshipViewModel _model;
            private readonly CreateCohortRequest _createCohortRequest;
            private readonly CreateCohortResponse _createCohortResponse;
            private readonly CreateCohortWithDraftApprenticeshipRequest _createCohortWithDraftApprenticeshipRequest;
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

            private CommitmentsV2.Api.Types.Responses.ValidateUlnOverlapResult _validateUlnOverlapResult;
            private Infrastructure.OuterApi.Responses.ValidateUlnOverlapOnStartDateQueryResult _validateUlnOverlapOnStartDateResult;


            public UnapprovedControllerTestFixture()
            {
                _autoFixture = new Fixture();

                _draftApprenticeshipHashedId = _autoFixture.Create<string>();
                _mediator = new Mock<IMediator>();
                _mockModelMapper = new Mock<IModelMapper>();
                _linkGenerator = new Mock<ILinkGenerator>();
                _encodingService = new Mock<IEncodingService>();
                _createCohortWithDraftApprenticeshipRequest =
                    _autoFixture.Create<CreateCohortWithDraftApprenticeshipRequest>();

                _model = new AddDraftApprenticeshipViewModel
                {
                    ProviderId = _autoFixture.Create<int>(),
                    EmployerAccountLegalEntityPublicHashedId = _autoFixture.Create<string>(),
                    AccountLegalEntityId = _autoFixture.Create<long>(),
                    ReservationId = _autoFixture.Create<Guid>()
                };

                _tempData = new Mock<ITempDataDictionary>();

                _createCohortRequest = new CreateCohortRequest();
                _mockModelMapper
                    .Setup(x => x.Map<CreateCohortRequest>(It.IsAny<AddDraftApprenticeshipViewModel>()))
                    .ReturnsAsync(_createCohortRequest);

                _mockModelMapper
                    .Setup(x => x.Map<CreateCohortWithDraftApprenticeshipRequest>(_model))
                    .ReturnsAsync(_createCohortWithDraftApprenticeshipRequest);

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
                    ProviderId = 2
                };

                _mediator.Setup(x => x.Send(It.IsAny<CreateCohortRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_createCohortResponse);

                _linkGeneratorRedirectUrl = _autoFixture.Create<string>();
                _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                    .Returns(_linkGeneratorRedirectUrl)
                    .Callback((string value) => _linkGeneratorParameter = value);

                var authorizationService = Mock.Of<IAuthorizationService>();

                _outerApiService = new Mock<IOuterApiService>();
                _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
                _commitmentsApiClient.Setup(x => x.ValidateUlnOverlap(It.IsAny<CommitmentsV2.Api.Types.Requests.ValidateUlnOverlapRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => _validateUlnOverlapResult);

                _outerApiService.Setup(x => x.ValidateUlnOverlapOnStartDate(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => _validateUlnOverlapOnStartDateResult);

                _mockModelMapper.Setup(x => x.Map<CreateCohortRequest>(It.IsAny<DraftApprenticeshipOverlapOptionViewModel>())).ReturnsAsync(() => new CreateCohortRequest());

                _controller = new CohortController(
                    _mediator.Object,
                    _mockModelMapper.Object,
                    _linkGenerator.Object,
                    _commitmentsApiClient.Object,
                    authorizationService,
                    _encodingService.Object,
                    _outerApiService.Object);
                _controller.TempData = _tempData.Object;
            }

            public UnapprovedControllerTestFixture SetupStartDraftOverlapOptions(OverlapOptions overlapOption)
            {
                _draftApprenticeshipOverlapOptionViewModel.OverlapOptions = overlapOption;
                return this;
            }

            public UnapprovedControllerTestFixture SetupStartDateOverlap(bool overlapStartDate, bool overlapEndDate)
            {
                _validateUlnOverlapOnStartDateResult = new Infrastructure.OuterApi.Responses.ValidateUlnOverlapOnStartDateQueryResult
                {
                    HasOverlapWithApprenticeshipId = 1,
                    HasStartDateOverlap = overlapStartDate
                };

                return this;
            }

            public UnapprovedControllerTestFixture SetupAddDraftApprenticeshipViewModelForStartDateOverlap()
            {
                _model.StartMonth = 1;
                _model.StartYear = 2022;
                _model.EndMonth = 1;
                _model.EndYear = 2023;
                _model.Uln = "XXXX";

                return this;
            }

            public async Task<UnapprovedControllerTestFixture> PostDraftApprenticeshipViewModel(string changeCourse = null, string changeDeliveryModel = null, string changePilotStatus = null)
            {
                _actionResult = await _controller.AddDraftApprenticeshipOrRoute(changeCourse, changeDeliveryModel, changePilotStatus, _model);
                return this;
            }

            public UnapprovedControllerTestFixture SetupHasOptions()
            {
                var draftApprenticeshipId = _autoFixture.Create<long>();

                _encodingService.Setup(x => x.Encode(draftApprenticeshipId, EncodingType.ApprenticeshipId))
                    .Returns(_draftApprenticeshipHashedId);
                
                _mediator.Setup(x => x.Send(It.IsAny<CreateCohortRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new CreateCohortResponse
                    {
                        CohortId = _autoFixture.Create<long>(),
                        CohortReference = _autoFixture.Create<string>(),
                        HasStandardOptions = true,
                        DraftApprenticeshipId = draftApprenticeshipId
                    });
                return this;
            }

            public UnapprovedControllerTestFixture VerifyCohortCreated()
            {
                //1. Verify that the viewmodel submitted was mapped
                _mockModelMapper.Verify(x => x.Map<CreateCohortRequest>(It.Is<AddDraftApprenticeshipViewModel>(m => m == _model)),Times.Once);
                //2. Verify that the mapper result (request) was sent
                _mediator.Verify(x => x.Send(It.Is<CreateCohortRequest>(r => r == _createCohortRequest), It.IsAny<CancellationToken>()), Times.Once);
                return this;
            }

            public UnapprovedControllerTestFixture VerifyUserRedirectedTo(string page)
            {
                _actionResult.VerifyReturnsRedirectToActionResult().WithActionName(page);
                return this;
            }

            public UnapprovedControllerTestFixture VerifyUserRedirectSelectOption()
            {
                _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("SelectOptions");
                var result = _actionResult as RedirectToActionResult;
                Assert.IsNotNull(result);
                result.RouteValues["DraftApprenticeshipHashedId"].Should().Be(_draftApprenticeshipHashedId);
                return this;
            }

            public void SetModelStartDate(string monthYear, bool setActual = false)
            {
                if (setActual)
                {
                    _model.StartDate = new MonthYearModel("");
                    _model.ActualStartDate = new MonthYearModel(monthYear);
                }
                else
                {
                    _model.StartDate = new MonthYearModel(monthYear);
                    _model.ActualStartDate = new MonthYearModel("");
                }
            }

            public void VerifyUserRedirectIsNotToRecognisePriorLearning()
            {
                _actionResult.VerifyReturnsRedirectToActionResult();
                var result = _actionResult as RedirectToActionResult;
                Assert.IsNotNull(result);
                Assert.AreNotSame("RecognisePriorLearning", result.ActionName);
            }

            public UnapprovedControllerTestFixture VerifyOverlappingTrainingDateRequestEmailSent()
            {
                _outerApiService.Verify(x => x.CreateOverlappingTrainingDateRequest(It.IsAny<CreateOverlappingTrainingDateApimRequest>()), Times.Once);
                return this;
            }

            public UnapprovedControllerTestFixture VerifyOverlappingTrainingDateRequestEmail_IsNotSent()
            {
                _outerApiService.Verify(x => x.CreateOverlappingTrainingDateRequest(It.IsAny<CreateOverlappingTrainingDateApimRequest>()), Times.Never);
                return this;
            }
        }
    }
}
