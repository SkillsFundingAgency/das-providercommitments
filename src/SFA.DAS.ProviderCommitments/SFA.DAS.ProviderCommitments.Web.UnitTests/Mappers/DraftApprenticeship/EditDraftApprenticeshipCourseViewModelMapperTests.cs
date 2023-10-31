using System;
using System.Linq;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeship
{
    [TestFixture]
    public class EditDraftApprenticeshipCourseViewModelMapperTests
    {
        private EditDraftApprenticeshipCourseViewModelMapper _mapper;
        private Mock<IOuterApiClient> _apiClient;
        private Mock<ITempDataStorageService> _tempDataStorageService;
        private DraftApprenticeshipRequest _request;
        private GetEditDraftApprenticeshipCourseResponse _apiResponse;
        private readonly Fixture _fixture = new();
        private EditDraftApprenticeshipViewModel _cacheModel;

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<DraftApprenticeshipRequest>();
            _apiResponse = _fixture.Create<GetEditDraftApprenticeshipCourseResponse>();
            _cacheModel = new EditDraftApprenticeshipViewModel(new DateTime(2000, 1, 1), null, null, null);

            _tempDataStorageService = new Mock<ITempDataStorageService>();
            _tempDataStorageService.Setup(x => x.RetrieveFromCache<EditDraftApprenticeshipViewModel>())
                .Returns(_cacheModel);

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetEditDraftApprenticeshipCourseResponse>(It.Is<GetEditDraftApprenticeshipCourseRequest>(r =>
                    r.DraftApprenticeshipId == _request.DraftApprenticeshipId
                    && r.CohortId == _request.CohortId
                    && r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_apiResponse);

            _mapper = new EditDraftApprenticeshipCourseViewModelMapper(_tempDataStorageService.Object, _apiClient.Object);
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
    }
}
