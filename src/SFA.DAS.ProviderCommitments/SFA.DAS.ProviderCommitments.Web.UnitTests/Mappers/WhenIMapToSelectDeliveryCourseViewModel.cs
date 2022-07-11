using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers

{
    [TestFixture]
    public class WhenIMapToSelectDeliveryCourseViewModel
    {
        private SelectDeliveryModelMapperHelper _mapper;
        private Mock<IApprovalsOuterApiClient> _outerApiClient;
        private ProviderCourseDeliveryModels _response;
        private long _providerId;
        private string _courseCode;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _providerId = fixture.Create<long>();
            _courseCode = fixture.Create<string>();
            _response = fixture.Create<ProviderCourseDeliveryModels>();

            _outerApiClient = new Mock<IApprovalsOuterApiClient>();
            _outerApiClient.Setup(x => x.GetProviderCourseDeliveryModels(_providerId, _courseCode, 0, It.IsAny<CancellationToken>())).ReturnsAsync(_response);

            _mapper = new SelectDeliveryModelMapperHelper(_outerApiClient.Object);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _mapper.Map(_providerId, _courseCode, 0, null);
            Assert.AreEqual(_courseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenDeliveryModelsAreReturnedCorrectly()
        {
            var result = await _mapper.Map(_providerId, _courseCode, 0, null);
            Assert.AreEqual(_response.DeliveryModels, result.DeliveryModels);
        }

        [TestCase(DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.Regular)]
        [TestCase(null)]
        public async Task ThenDeliveryModelisMappedCorrectly(DeliveryModel? dm)
        {
            var result = await _mapper.Map(_providerId, _courseCode, 0, dm);
            Assert.AreEqual(dm, result.DeliveryModel);
        }
    }
}
