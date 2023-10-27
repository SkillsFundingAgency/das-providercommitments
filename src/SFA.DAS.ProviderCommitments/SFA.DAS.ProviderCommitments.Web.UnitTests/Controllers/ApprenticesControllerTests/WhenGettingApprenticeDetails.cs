using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingApprenticeDetails
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new GetApprenticeDetailsFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new GetApprenticeDetailsFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel().WithModel<DetailsViewModel>();
        }
    }

    public class GetApprenticeDetailsFixture
    {
        public ApprenticeController Sut { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly DetailsViewModel _viewModel;
        private readonly DetailsRequest _request;
        private readonly long _providerId;

        public GetApprenticeDetailsFixture()
        {
            var fixture = new Fixture();
            _providerId = 123;
            _request = new DetailsRequest { ProviderId = _providerId, ApprenticeshipHashedId = "XYZ" };
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = fixture.Create<DetailsViewModel>();

            _modelMapperMock
                .Setup(x => x.Map<DetailsViewModel>(_request))
                .ReturnsAsync(_viewModel);

            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<DetailsViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.Details(_request);
    }
}
