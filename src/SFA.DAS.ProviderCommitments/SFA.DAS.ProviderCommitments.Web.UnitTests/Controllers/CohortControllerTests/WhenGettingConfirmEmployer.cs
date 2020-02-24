using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using AutoFixture;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Requests.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingConfirmEmployer
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new GetConfirmEmployerFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new GetConfirmEmployerFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel().WithModel<ConfirmEmployerViewModel>();
        }
    }

    public class GetConfirmEmployerFixture
    {
        public CohortController Sut { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly ConfirmEmployerViewModel _viewModel;
        private readonly ConfirmEmployerRequest _request;
        private readonly long _providerId;

        public GetConfirmEmployerFixture()
        {
            var fixture = new Fixture();
            _request = new ConfirmEmployerRequest { ProviderId = _providerId, EmployerAccountLegalEntityPublicHashedId = "XYZ" };
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = fixture.Create<ConfirmEmployerViewModel>();
            _providerId = 123;

            _modelMapperMock
                .Setup(x => x.Map<ConfirmEmployerViewModel>(_request))
                .ReturnsAsync(_viewModel);
            
            Sut = new CohortController(Mock.Of<IMediator>(),_modelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>());
        }

        public GetConfirmEmployerFixture WithModelStateErrors()
        {
            Sut.ControllerContext.ModelState.AddModelError("TestError", "Test Error");
            return this;
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ConfirmEmployerViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.ConfirmEmployer(_request);
    }
}
