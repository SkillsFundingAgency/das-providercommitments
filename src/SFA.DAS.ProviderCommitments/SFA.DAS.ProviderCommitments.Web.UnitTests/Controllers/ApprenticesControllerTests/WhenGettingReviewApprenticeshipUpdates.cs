using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingReviewApprenticeshipUpdates
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new GetReviewApprenticeshipUpdatesFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new GetReviewApprenticeshipUpdatesFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel().WithModel<ReviewApprenticeshipUpdatesViewModel>();
        }
    }

    public class GetReviewApprenticeshipUpdatesFixture
    {
        public ApprenticeController Sut { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly ReviewApprenticeshipUpdatesViewModel _viewModel;
        private readonly ReviewApprenticeshipUpdatesRequest _request;
        private readonly long _providerId;

        public GetReviewApprenticeshipUpdatesFixture()
        {
            var fixture = new Fixture();
            _providerId = 123;
            _request = new ReviewApprenticeshipUpdatesRequest { ProviderId = _providerId, ApprenticeshipHashedId = "XYZ" };
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = fixture.Create<ReviewApprenticeshipUpdatesViewModel>();

            _modelMapperMock
                .Setup(x => x.Map<ReviewApprenticeshipUpdatesViewModel>(_request))
                .ReturnsAsync(_viewModel);

            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ReviewApprenticeshipUpdatesViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.ReviewApprenticeshipUpdates(_request);
    }
}
