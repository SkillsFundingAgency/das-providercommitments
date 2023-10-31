using System.Linq;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeship
{
    [TestFixture]
    public class AddDraftApprenticeshipCourseViewModelMapperTests
    {
        private AddDraftApprenticeshipCourseViewModelMapper _mapper;
        private Mock<IOuterApiClient> _apiClient;
        private Mock<IAuthorizationService> _authorizationService;
        private ReservationsAddDraftApprenticeshipRequest _request;
        private GetAddDraftApprenticeshipCourseResponse _apiResponse;
        private readonly Fixture _fixture = new();

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<ReservationsAddDraftApprenticeshipRequest>();
            _apiResponse = _fixture.Create<GetAddDraftApprenticeshipCourseResponse>();

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetAddDraftApprenticeshipCourseResponse>(It.Is<GetAddDraftApprenticeshipCourseRequest>(r =>
                    r.CohortId == _request.CohortId
                    && r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_apiResponse);

            _authorizationService = new Mock<IAuthorizationService>();
            _authorizationService.Setup(x => x.IsAuthorizedAsync(ProviderFeature.FlexiblePaymentsPilot))
                .ReturnsAsync(false);

            _mapper = new AddDraftApprenticeshipCourseViewModelMapper(_apiClient.Object, _authorizationService.Object);
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
        public async Task IsOnFlexiPaymentsPilot_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.IsFalse(result.IsOnFlexiPaymentsPilot);
        }

        [Test]
        public async Task CourseCode_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_request.CourseCode, result.CourseCode);
        }
    }
}
