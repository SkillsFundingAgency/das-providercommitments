﻿using System;
using System.Security.Claims;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenPostingDetails
    {
        private WhenPostingDetailsFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenPostingDetailsFixture();
        }

        [Test]
        public async Task And_User_Selected_Send_Then_Cohort_Is_Sent_To_Provider()
        {
            await _fixture.Post(CohortDetailsOptions.Send);
            _fixture.VerifyCohortSubmission();
        }

        [Test]
        public async Task And_User_Selected_Send_Then_User_Is_Redirected_To_Confirmation_Page()
        {
            await _fixture.Post(CohortDetailsOptions.Send);
            _fixture.VerifyRedirectedToSendConfirmation();
        }

        [Test]
        public async Task And_User_Selected_Approve_Then_Cohort_Is_Approved_By_Employer()
        {
            await _fixture.Post(CohortDetailsOptions.Approve);
            _fixture.VerifyCohortSubmission();
        }

        [Test]
        public async Task And_User_Selected_Approve_Then_User_Is_Redirected_To_Confirmation_Page()
        {
            await _fixture.Post(CohortDetailsOptions.Approve);
            _fixture.VerifyRedirectedToAcknowledgement();
        }

        [Test]
        public void And_User_DoesNot_Have_Permission_To_Approve_Then_UnAuthorizedException_IsThrown()
        {
            _fixture.SetUpIsAuthorized(false);
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _fixture.Post(CohortDetailsOptions.Approve));
        }

        [Test]
        public void And_User_DoesNot_Have_Permission_To_Send_Then_UnAuthorizedException_IsThrown()
        {
            _fixture.SetUpIsAuthorized(false);
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _fixture.Post(CohortDetailsOptions.Send));
        }

        [Test]
        public void And_User_Have_Permission_To_Approve_Then_No_Exception_Is_Thrown()
        {
            _fixture.SetUpIsAuthorized(true);
            Assert.DoesNotThrowAsync(() => _fixture.Post(CohortDetailsOptions.Approve));
        }

        [Test]
        public void And_User_Have_Permission_To_Send_Then_No_Exception_Is_Thrown()
        {
            _fixture.SetUpIsAuthorized(true);
            Assert.DoesNotThrowAsync(() => _fixture.Post(CohortDetailsOptions.Send));
        }

        [Test]
        public async Task And_User_Selected_ApprenticeRequest_Then_User_Is_Redirected_To_ApprenticeRequest()
        {
            await _fixture.Post(CohortDetailsOptions.ApprenticeRequest);
            _fixture.VerifyRedirectedToApprenticeRequest();
        }

        public class WhenPostingDetailsFixture
        {
            private readonly CohortController _controller;
            private IActionResult _result;
            private readonly Mock<IModelMapper> _modelMapper;
            private readonly DetailsViewModel _viewModel;
            private readonly Mock<IPolicyAuthorizationWrapper> _policyAuthorizationWrapper;

            public WhenPostingDetailsFixture()
            {
                var autoFixture = new Fixture();

                var commitmentsApiClient = new Mock<ICommitmentsApiClient>();
                _policyAuthorizationWrapper = new Mock<IPolicyAuthorizationWrapper>();
                var linkGenerator = new Mock<ILinkGenerator>();

                var cohortId = autoFixture.Create<long>();
                var providerId = autoFixture.Create<long>();
                var accountLegalEntityHashedId = autoFixture.Create<string>();

                _viewModel = new DetailsViewModel
                {
                    CohortId = cohortId,
                    ProviderId = providerId,
                    AccountLegalEntityHashedId = accountLegalEntityHashedId
                };

                var sendCohortApiRequest = new SendCohortRequest();
                var approveCohortApiRequest = new ApproveCohortRequest();

                _modelMapper = new Mock<IModelMapper>();
                _modelMapper.Setup(x => x.Map<SendCohortRequest>(It.Is<DetailsViewModel>(vm => vm == _viewModel)))
                    .ReturnsAsync(sendCohortApiRequest);

                _modelMapper.Setup(x => x.Map<ApproveCohortRequest>(It.Is<DetailsViewModel>(vm => vm == _viewModel)))
                    .ReturnsAsync(approveCohortApiRequest);

                commitmentsApiClient.Setup(x => x.SendCohort(It.Is<long>(c => c == cohortId),
                        It.Is<SendCohortRequest>(r => r == sendCohortApiRequest), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                commitmentsApiClient.Setup(x => x.ApproveCohort(It.Is<long>(c => c == cohortId),
                        It.Is<ApproveCohortRequest>(r => r == approveCohortApiRequest), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
                _policyAuthorizationWrapper.Setup(x => x.IsAuthorized(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>())).ReturnsAsync(true);

                var linkGeneratorResult = autoFixture.Create<string>();

                linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                    .Returns(linkGeneratorResult);

                _controller = new CohortController(Mock.Of<IMediator>(),
                    _modelMapper.Object,
                    linkGenerator.Object,
                    commitmentsApiClient.Object, 
                    Mock.Of<IEncodingService>(),
                    Mock.Of<IOuterApiService>(),
                    Mock.Of<IAuthorizationService>());
            }

            public async Task Post(CohortDetailsOptions option)
            {
                _viewModel.Selection = option;
              
                _result = await _controller.Details(_policyAuthorizationWrapper.Object, _viewModel);
            }

            public void VerifyCohortSubmission()
            {
                _modelMapper.Verify(x => x.Map<AcknowledgementRequest>(It.Is<DetailsViewModel>(vm => vm == _viewModel)), Times.Once);
            }

            public void VerifyRedirectedToSendConfirmation()
            {
                Assert.That(_result, Is.InstanceOf<RedirectToActionResult>());
                var redirect = (RedirectToActionResult)_result;
                Assert.That(redirect.ActionName, Is.EqualTo("Acknowledgement"));
            }

            public void VerifyRedirectedToAcknowledgement()
            {
                Assert.That(_result, Is.InstanceOf<RedirectToActionResult>());
                var redirect = (RedirectToActionResult)_result;
                Assert.That(redirect.ActionName, Is.EqualTo("Acknowledgement"));
            }

            public void VerifyRedirectedToApprenticeRequest()
            {
                Assert.That(_result, Is.InstanceOf<RedirectToActionResult>());
                var redirect = (RedirectToActionResult)_result;
                Assert.That(redirect.ActionName, Is.EqualTo("Review"));
            }

            internal void SetUpIsAuthorized(bool isAuthorized)
            {
                _policyAuthorizationWrapper.Setup(x => x.IsAuthorized(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>()))
                    .ReturnsAsync(isAuthorized);
            }
        }
    }
}
