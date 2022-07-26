﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers
{
    [TestFixture]
    public class WhenIMapToSelectDeliveryCourseViewModel
    {
        private SelectDeliveryModelMapperHelper _mapper;
        private Mock<IApprovalsOuterApiClient> _outerApiClient;
        private Mock<IEncodingService> _encodingService;
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
            _outerApiClient.Setup(x => x.GetProviderCourseDeliveryModels(_providerId, _courseCode, 1234, It.IsAny<CancellationToken>())).ReturnsAsync(_response);
            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.PublicAccountLegalEntityId))
                .Returns(1234);
            _mapper = new SelectDeliveryModelMapperHelper(_outerApiClient.Object, _encodingService.Object);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _mapper.Map(_providerId, _courseCode, 1234, null);
            Assert.AreEqual(_courseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenDeliveryModelsAreReturnedCorrectly()
        {
            var result = await _mapper.Map(_providerId, _courseCode, 1234, null);
            Assert.AreEqual(_response.DeliveryModels, result.DeliveryModels);
        }

        [TestCase(DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.Regular)]
        [TestCase(null)]
        public async Task ThenDeliveryModelisMappedCorrectly(DeliveryModel? dm)
        {
            var result = await _mapper.Map(_providerId, _courseCode, 1234, dm);
            Assert.AreEqual(dm, result.DeliveryModel);
        }
    }

    [TestFixture]
    public class WhenICheckForMultipleDeliveryCourses
    {
        private SelectDeliveryModelMapperHelper _mapper;
        private Mock<IApprovalsOuterApiClient> _outerApiClient;
        private Mock<IEncodingService> _encodingService;
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
            _outerApiClient.Setup(x => x.GetProviderCourseDeliveryModels(_providerId, _courseCode, 1234, It.IsAny<CancellationToken>())).ReturnsAsync(_response);
            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.PublicAccountLegalEntityId))
                .Returns(1234);

            _mapper = new SelectDeliveryModelMapperHelper(_outerApiClient.Object, _encodingService.Object);
        }

        [Test]
        public async Task ThenReturnsTrueWhenMultipleDeliveryModelsExist()
        {
            var result = await _mapper.HasMultipleDeliveryModels(_providerId, _courseCode, "PALID");
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ThenReturnsFalseWhenMultipleDeliveryModelsDoNotExist()
        {
            _response.DeliveryModels = new List<DeliveryModel> {DeliveryModel.Regular};
            var result = await _mapper.HasMultipleDeliveryModels(_providerId, _courseCode, "PALID");
            Assert.IsFalse(result);
        }
    }
}
