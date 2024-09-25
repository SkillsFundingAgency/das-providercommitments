using SFA.DAS.ProviderCommitments.Web.Services;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

[TestFixture]
public class BaseApprenticeshipRequestFromEditApprenticeshipCourseViewModelMapperTests
{
    private BaseApprenticeshipRequestFromEditApprenticeshipCourseViewModelMapper _mapper;
    private Mock<ITempDataStorageService> _tempDataStorageService;
    private EditApprenticeshipCourseViewModel _request;
    private readonly Fixture _fixture = new();
    private EditApprenticeshipRequestViewModel _cacheModel;
    private const string ViewModelForEdit = "ViewModelForEdit";

    [SetUp]
    public void Setup()
    {
        _request = _fixture.Create<EditApprenticeshipCourseViewModel>();
        _cacheModel = _fixture.Build<EditApprenticeshipRequestViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
            .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x => x.StartDate)
            .Without(x => x.EndDate).Without(x => x.EndMonth).Without(x => x.EndMonth).Without(x => x.EndYear)
            .Without(x => x.EndMonth).Without(x => x.EndYear).Without(x => x.EndDate)
            .Create();


        _tempDataStorageService = new Mock<ITempDataStorageService>();
        _tempDataStorageService.Setup(x => x.RetrieveFromCache<EditApprenticeshipRequestViewModel>(ViewModelForEdit))
            .Returns(_cacheModel);

        _mapper = new BaseApprenticeshipRequestFromEditApprenticeshipCourseViewModelMapper(_tempDataStorageService.Object);
    }

    [Test]
    public async Task ApprenticeshipHashedId_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.ApprenticeshipHashedId.Should().Be(_cacheModel.ApprenticeshipHashedId);
    }

    [Test]
    public async Task ProviderId_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.ProviderId.Should().Be(_cacheModel.ProviderId);
    }
}