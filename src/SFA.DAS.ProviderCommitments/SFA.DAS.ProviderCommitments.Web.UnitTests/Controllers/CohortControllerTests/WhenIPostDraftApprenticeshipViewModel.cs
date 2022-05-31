using System;
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
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
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
        public async Task UserIsRedirectedToRecognisePriorLearningPageWhenStartDateIsAfterActivationDate()
        {
            _fixture.SetModelStartDate("012023");
            await _fixture.PostDraftApprenticeshipViewModel();
            _fixture.VerifyUserRedirectedTo("RecognisePriorLearning");
        }

        [Test]
        public async Task UserIsNotRedirectedToRecognisePriorLearningPageWhenStartDateIsBeforeActivationDate()
        {
            _fixture.SetModelStartDate("012022");
            await _fixture.PostDraftApprenticeshipViewModel();
            _fixture.VerifyUserRedirectIsNotToRecognisePriorLearning();
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
            private IActionResult _actionResult;
            private readonly string _linkGeneratorRedirectUrl;
            private string _linkGeneratorParameter;
            private Fixture _autoFixture;
            private readonly Mock<IEncodingService> _encodingService;
            private readonly Mock<ITempDataDictionary> _tempData;
            private readonly string _draftApprenticeshipHashedId;
            private readonly RecognitionOfPriorLearningConfiguration _rplConfiguration;

            public UnapprovedControllerTestFixture()
            {
                _autoFixture = new Fixture();

                _draftApprenticeshipHashedId = _autoFixture.Create<string>();
                _mediator = new Mock<IMediator>();
                _mockModelMapper = new Mock<IModelMapper>();
                _linkGenerator = new Mock<ILinkGenerator>();
                _encodingService = new Mock<IEncodingService>();

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

                _createCohortResponse = new CreateCohortResponse
                {
                    CohortId = _autoFixture.Create<long>(),
                    CohortReference = _autoFixture.Create<string>(),
                    HasStandardOptions = false,
                    DraftApprenticeshipId = _autoFixture.Create<long>(),
                };

                _mediator.Setup(x => x.Send(It.IsAny<CreateCohortRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_createCohortResponse);

                _linkGeneratorRedirectUrl = _autoFixture.Create<string>();
                _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                    .Returns(_linkGeneratorRedirectUrl)
                    .Callback((string value) => _linkGeneratorParameter = value);

                _rplConfiguration = new RecognitionOfPriorLearningConfiguration
                {
                    MandateRplAfter = new DateTime(2022, 08, 01)
                };

                _controller = new CohortController(_mediator.Object, _mockModelMapper.Object, _linkGenerator.Object, Mock.Of<ICommitmentsApiClient>(), 
                            Mock.Of<IAuthorizationService>(), _encodingService.Object,  Mock.Of<IOuterApiService>(), _rplConfiguration);
                _controller.TempData = _tempData.Object;
            }

            public async Task<UnapprovedControllerTestFixture> PostDraftApprenticeshipViewModel(string changeCourse = null, string changeDeliveryModel = null)
            {
                _actionResult = await _controller.AddDraftApprenticeshipOrRoute(changeCourse, changeDeliveryModel, _model);
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

            public void SetModelStartDate(string monthYear)
            {
                _model.StartDate = new MonthYearModel(monthYear);
            }

            public void VerifyUserRedirectIsNotToRecognisePriorLearning()
            {
                _actionResult.VerifyReturnsRedirectToActionResult();
                var result = _actionResult as RedirectToActionResult;
                Assert.IsNotNull(result);
                Assert.AreNotSame(result.ActionName, "RecognisePriorLearning");
            }
        }
    }
}
