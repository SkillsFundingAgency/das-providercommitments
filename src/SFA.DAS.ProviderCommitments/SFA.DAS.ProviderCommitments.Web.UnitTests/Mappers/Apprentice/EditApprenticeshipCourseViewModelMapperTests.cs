using System;
using System.Linq;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class EditApprenticeshipCourseViewModelMapperTests
    {
        private EditApprenticeshipCourseViewModelMapper _mapper;
        private Mock<IOuterApiClient> _apiClient;
        private Mock<ITempDataStorageService> _tempDataStorageService;
        private EditApprenticeshipRequest _request;
        private GetEditApprenticeshipCourseResponse _apiResponse;
        private readonly Fixture _fixture = new();
        private EditApprenticeshipRequestViewModel _cacheModel;
        private const string ViewModelForEdit = "ViewModelForEdit";

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<EditApprenticeshipRequest>();
            _apiResponse = _fixture.Create<GetEditApprenticeshipCourseResponse>();
            _cacheModel = new EditApprenticeshipRequestViewModel(new DateTime(2000, 1, 1), null, null, null);

            _tempDataStorageService = new Mock<ITempDataStorageService>();
            _tempDataStorageService.Setup(x => x.RetrieveFromCache<EditApprenticeshipRequestViewModel>(ViewModelForEdit))
                .Returns(_cacheModel);

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetEditApprenticeshipCourseResponse>(It.Is<GetEditApprenticeshipCourseRequest>(r =>
                    r.ApprenticeshipId == _request.ApprenticeshipId
                    && r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_apiResponse);

            _mapper = new EditApprenticeshipCourseViewModelMapper(_tempDataStorageService.Object, _apiClient.Object);
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
