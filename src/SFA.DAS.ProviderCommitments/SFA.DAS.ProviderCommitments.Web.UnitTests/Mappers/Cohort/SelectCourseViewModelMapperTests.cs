using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using System.Linq;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using System;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class SelectCourseViewModelMapperTests
    {
        private SelectCourseViewModelMapper _mapper;
        private Mock<IOuterApiClient> _apiClient;
        private SelectCourseRequest _request;
        private GetAddDraftApprenticeshipCourseResponse _apiResponse;
        private Mock<ICacheStorageService> _cacheService;
        private CreateCohortCacheItem _cacheItem;
        private readonly Fixture _fixture = new();

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<SelectCourseRequest>();
            _apiResponse = _fixture.Create<GetAddDraftApprenticeshipCourseResponse>();

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetAddDraftApprenticeshipCourseResponse>(It.Is<GetAddDraftApprenticeshipCourseRequest>(r =>
                    r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_apiResponse);

            _cacheItem = _fixture.Create<CreateCohortCacheItem>();
            _cacheService = new Mock<ICacheStorageService>();
            _cacheService.Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(It.IsAny<Guid>()))
                .ReturnsAsync(_cacheItem);

            _mapper = new SelectCourseViewModelMapper(_apiClient.Object, _cacheService.Object);
        }

        [Test]
        public async Task EmployerName_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.EmployerName, result.EmployerName);
        }

        [Test]
        public async Task ProviderId_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_request.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ShowManagingStandardsContent_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.IsMainProvider, result.ShowManagingStandardsContent);
        }

        [Test]
        public async Task Standards_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.IsTrue(TestHelper.EnumerablesAreEqual(_apiResponse.Standards.ToList(), result.Standards.ToList()));
        }

        [Test]
        public async Task CourseCode_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_cacheItem.CourseCode, result.CourseCode);
        }
    }
}
