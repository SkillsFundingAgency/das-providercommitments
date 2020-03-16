using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class DatesViewModelMapperTests
    {
        private DatesViewModelMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new DatesViewModelMapperFixture();
        }


        [Test]
        public async Task ThenCallsApiClient()
        {
            await _fixture.Act();

            _fixture.Verify_ICommitmentsApiClient_WasCalled(Times.Once());
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Request.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenProviderIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Request.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Request.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenStopDateIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Response.StopDate, result.StopDate);
        }

    }

    public class DatesViewModelMapperFixture
    {
        private readonly DatesViewModel _viewModel;
        private readonly Mock<ICommitmentsApiClient> _commitmentsApiClientMock;
        private readonly DatesViewModelMapper _sut;

        public DatesRequest Request { get; }
        public GetApprenticeshipResponse Response { get; }
        public DatesViewModelMapperFixture()
        {
            Request = new DatesRequest
            {
                ApprenticeshipHashedId = "SF45G54",
                ApprenticeshipId = 234,
                ProviderId = 645621,
                EmployerAccountLegalEntityPublicHashedId = "GD35SD35"
            };
            Response = new GetApprenticeshipResponse
            {
                StopDate = DateTime.UtcNow.AddDays(-5)
            };
            _commitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClientMock
                .Setup(x => x.GetApprenticeship(Request.ApprenticeshipId, CancellationToken.None))
                .ReturnsAsync(Response);
            _sut = new DatesViewModelMapper(_commitmentsApiClientMock.Object);
        }

        public Task<DatesViewModel> Act() => _sut.Map(Request);

        public void Verify_ICommitmentsApiClient_WasCalled(Times times)
        {
            _commitmentsApiClientMock
                .Verify(x => x.GetApprenticeship(Request.ApprenticeshipId, CancellationToken.None), times);
        }
    }
}
