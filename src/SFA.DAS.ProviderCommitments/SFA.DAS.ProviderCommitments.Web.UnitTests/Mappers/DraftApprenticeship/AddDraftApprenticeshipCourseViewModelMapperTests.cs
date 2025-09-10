using System;
using System.Linq;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeship;

[TestFixture]
public class AddDraftApprenticeshipCourseViewModelMapperTests
{
    private AddDraftApprenticeshipCourseViewModelMapper _mapper;
    private Mock<IOuterApiClient> _apiClient;
    private Mock<ICacheStorageService> _cacheService;
    private ReservationsAddDraftApprenticeshipRequest _request;
    private GetAddDraftApprenticeshipCourseResponse _apiResponse;
    private ReservationsAddDraftApprenticeshipRequest _cacheItem;
    private readonly Fixture _fixture = new();

    [SetUp]
    public void Setup()
    {
        _request = _fixture.Create<ReservationsAddDraftApprenticeshipRequest>();
        _apiResponse = _fixture.Create<GetAddDraftApprenticeshipCourseResponse>();
        _cacheItem = _fixture.Create<ReservationsAddDraftApprenticeshipRequest>();

        _apiClient = new Mock<IOuterApiClient>();
        _apiClient.Setup(x => x.Get<GetAddDraftApprenticeshipCourseResponse>(It.Is<GetAddDraftApprenticeshipCourseRequest>(r =>
                r.CohortId == _request.CohortId
                && r.ProviderId == _request.ProviderId)))
            .ReturnsAsync(_apiResponse);

        _cacheService = new Mock<ICacheStorageService>();
        _cacheService.Setup(x => x.RetrieveFromCache<ReservationsAddDraftApprenticeshipRequest>(_request.CacheKey.Value)).ReturnsAsync(_cacheItem);

        _mapper = new AddDraftApprenticeshipCourseViewModelMapper(_apiClient.Object, _cacheService.Object);
    }

    [Test]
    public async Task EmployerName_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.EmployerName.Should().Be(_apiResponse.EmployerName);
    }

    [Test]
    public async Task ProviderId_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.ProviderId.Should().Be(_request.ProviderId);
    }
        
    [Test]
    public async Task ReservationId_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.ReservationId.Should().Be(_request.ReservationId);
    }

    [Test]
    public async Task ShowManagingStandardsContent_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.ShowManagingStandardsContent.Should().Be(_apiResponse.IsMainProvider);
    }

    [Test]
    public async Task Standards_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        _apiResponse.Standards.ToList().Should().BeEquivalentTo(result.Standards.ToList());
    }

    [Test]
    public async Task CourseCode_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.CourseCode.Should().Be(_request.CourseCode);
    }

    [Test]
    public async Task CourseCode_Is_Mapped_From_Cache_If_CourseCode_Is_Null()
    {
        _request.CourseCode = null;
        var result = await _mapper.Map(_request);
        result.CourseCode.Should().Be(_cacheItem.CourseCode);
    }
}