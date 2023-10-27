using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using System;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using IAuthorizationService = SFA.DAS.Authorization.Services.IAuthorizationService;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
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
        public void And_User_Doesnot_Have_Permission_To_Approve_Then_UnAuthorizedException_IsThrown()
        {
            _fixture.SetUpIsAuthorized(false);
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _fixture.Post(CohortDetailsOptions.Approve));
        }

        [Test]
        public void And_User_Doesnot_Have_Permission_To_Send_Then_UnAuthorizedException_IsThrown()
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
            private readonly Mock<ICommitmentsApiClient> _commitmentsApiClient;
            private Mock<IModelMapper> _modelMapper;

            private readonly DetailsViewModel _viewModel;
            private readonly long _cohortId;
            private readonly long _providerId;
            private readonly string _accountLegalEntityHashedId;
            private readonly string _linkGeneratorResult;
            private readonly SendCohortRequest _sendCohortApiRequest;
            private readonly ApproveCohortRequest _approveCohortApiRequest;
            private readonly Mock<IPolicyAuthorizationWrapper> _policyAuthorizationWrapper;

            public WhenPostingDetailsFixture()
            {
                var autoFixture = new Fixture();

                _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
                _policyAuthorizationWrapper = new Mock<IPolicyAuthorizationWrapper>();
                var linkGenerator = new Mock<ILinkGenerator>();

                _cohortId = autoFixture.Create<long>();
                _providerId = autoFixture.Create<long>();
                _accountLegalEntityHashedId = autoFixture.Create<string>();

                _viewModel = new DetailsViewModel
                {
                    CohortId = _cohortId,
                    ProviderId = _providerId,
                    AccountLegalEntityHashedId = _accountLegalEntityHashedId
                };

                _sendCohortApiRequest = new SendCohortRequest();
                _approveCohortApiRequest = new ApproveCohortRequest();

                _modelMapper = new Mock<IModelMapper>();
                _modelMapper.Setup(x => x.Map<SendCohortRequest>(It.Is<DetailsViewModel>(vm => vm == _viewModel)))
                    .ReturnsAsync(_sendCohortApiRequest);

                _modelMapper.Setup(x => x.Map<ApproveCohortRequest>(It.Is<DetailsViewModel>(vm => vm == _viewModel)))
                    .ReturnsAsync(_approveCohortApiRequest);

                _commitmentsApiClient.Setup(x => x.SendCohort(It.Is<long>(c => c == _cohortId),
                        It.Is<SendCohortRequest>(r => r == _sendCohortApiRequest), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                _commitmentsApiClient.Setup(x => x.ApproveCohort(It.Is<long>(c => c == _cohortId),
                        It.Is<ApproveCohortRequest>(r => r == _approveCohortApiRequest), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
                _policyAuthorizationWrapper.Setup(x => x.IsAuthorized(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<string>())).ReturnsAsync(true);

                _linkGeneratorResult = autoFixture.Create<string>();

                linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                    .Returns(_linkGeneratorResult);

                _controller = new CohortController(Mock.Of<IMediator>(),
                    _modelMapper.Object,
                    linkGenerator.Object,
                    _commitmentsApiClient.Object, 
                    Mock.Of<IEncodingService>(),
                    Mock.Of<IOuterApiService>());
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
                Assert.IsInstanceOf<RedirectToActionResult>(_result);
                var redirect = (RedirectToActionResult)_result;
                Assert.AreEqual("Acknowledgement", redirect.ActionName);
            }

            public void VerifyRedirectedToAcknowledgement()
            {
                Assert.IsInstanceOf<RedirectToActionResult>(_result);
                var redirect = (RedirectToActionResult)_result;
                Assert.AreEqual("Acknowledgement", redirect.ActionName);
            }

            public void VerifyRedirectedToApprenticeRequest()
            {
                Assert.IsInstanceOf<RedirectToActionResult>(_result);
                var redirect = (RedirectToActionResult)_result;
                Assert.AreEqual("Review", redirect.ActionName);
            }

            internal void SetUpIsAuthorized(bool isAuhtorized)
            {
                _policyAuthorizationWrapper.Setup(x => x.IsAuthorized(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<string>()))
                    .ReturnsAsync(isAuhtorized);
            }
        }
    }
}
