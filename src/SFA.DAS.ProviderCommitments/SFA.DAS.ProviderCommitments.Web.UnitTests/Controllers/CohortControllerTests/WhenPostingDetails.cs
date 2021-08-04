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
            _fixture.VerifyCohortSentToProvider();
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
            _fixture.VerifyCohortApprovedByProvider();
        }

        [Test]
        public async Task And_User_Selected_Approve_Then_User_Is_Redirected_To_Confirmation_Page()
        {
            await _fixture.Post(CohortDetailsOptions.Approve);
            _fixture.VerifyRedirectedToAcknowledgement();
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

            private readonly DetailsViewModel _viewModel;
            private readonly long _cohortId;
            private readonly long _providerId;
            private readonly string _accountLegalEntityHashedId;
            private readonly string _linkGeneratorResult;
            private readonly SendCohortRequest _sendCohortApiRequest;
            private readonly ApproveCohortRequest _approveCohortApiRequest;

            public WhenPostingDetailsFixture()
            {
                var autoFixture = new Fixture();

                var modelMapper = new Mock<IModelMapper>();
                _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
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

                modelMapper.Setup(x => x.Map<SendCohortRequest>(It.Is<DetailsViewModel>(vm => vm == _viewModel)))
                    .ReturnsAsync(_sendCohortApiRequest);

                modelMapper.Setup(x => x.Map<ApproveCohortRequest>(It.Is<DetailsViewModel>(vm => vm == _viewModel)))
                    .ReturnsAsync(_approveCohortApiRequest);

                _commitmentsApiClient.Setup(x => x.SendCohort(It.Is<long>(c => c == _cohortId),
                        It.Is<SendCohortRequest>(r => r == _sendCohortApiRequest), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                _commitmentsApiClient.Setup(x => x.ApproveCohort(It.Is<long>(c => c == _cohortId),
                        It.Is<ApproveCohortRequest>(r => r == _approveCohortApiRequest), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                _linkGeneratorResult = autoFixture.Create<string>();

                linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                    .Returns(_linkGeneratorResult);

                _controller = new CohortController(Mock.Of<IMediator>(),
                     modelMapper.Object,
                     linkGenerator.Object,
                    _commitmentsApiClient.Object);
            }

            public async Task Post(CohortDetailsOptions option)
            {
                _viewModel.Selection = option;
                _result = await _controller.Details(_viewModel);
            }

            public void VerifyCohortSentToProvider()
            {
                _commitmentsApiClient.Verify(x => x.SendCohort(It.Is<long>(c => c == _cohortId),
                        It.Is<SendCohortRequest>(r => r == _sendCohortApiRequest),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
            }

            public void VerifyRedirectedToSendConfirmation()
            {
                Assert.IsInstanceOf<RedirectToActionResult>(_result);
                var redirect = (RedirectToActionResult)_result;
                Assert.AreEqual("Acknowledgement", redirect.ActionName);
            }

            public void VerifyCohortApprovedByProvider()
            {
                _commitmentsApiClient.Verify(x => x.ApproveCohort(It.Is<long>(c => c == _cohortId),
                        It.Is<ApproveCohortRequest>(r => r == _approveCohortApiRequest),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
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
                Assert.AreEqual("Cohorts", redirect.ActionName);
            }
        }
    }
}
