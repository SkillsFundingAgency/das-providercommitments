using System.Threading;
using AutoFixture;
using NUnit.Framework;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers

{
    [TestFixture]
    public class WhenIMapSelectDeliveryModelViewModelMapperFromReservationsAddDraftApprenticeshipRequest
    {
        private SelectDeliveryModelViewModelMapperFromReservationsAddDraftApprenticeshipRequestMapper _mapper;
        private Mock<ISelectDeliveryModelMapperHelper> _helper;
        private SelectDeliveryModelViewModel _model;
        private ReservationsAddDraftApprenticeshipRequest _request;
        private Mock<ICommitmentsApiClient> _commitmentsApiClient;
        private GetCohortResponse _getCohortResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Create<ReservationsAddDraftApprenticeshipRequest>();
            _model = fixture.Create<SelectDeliveryModelViewModel>();

            _helper = new Mock<ISelectDeliveryModelMapperHelper>();
            _helper.Setup(x => x.Map(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DeliveryModel?>(), It.IsAny<bool?>())).ReturnsAsync(_model);

            _getCohortResponse = fixture.Create<GetCohortResponse>();
            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _getCohortResponse );

            _mapper = new SelectDeliveryModelViewModelMapperFromReservationsAddDraftApprenticeshipRequestMapper(_helper.Object, _commitmentsApiClient.Object);
        }

        [Test]
        public async Task TheParamsArePassedInCorrectly()
        {
            await _mapper.Map(_request);
            _helper.Verify(x=>x.Map(_request.ProviderId, _request.CourseCode, _getCohortResponse.AccountLegalEntityId, _request.DeliveryModel, null));
       }

        [Test]
        public async Task ThenModelIsReturned()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_model, result);
        }
    }
}
