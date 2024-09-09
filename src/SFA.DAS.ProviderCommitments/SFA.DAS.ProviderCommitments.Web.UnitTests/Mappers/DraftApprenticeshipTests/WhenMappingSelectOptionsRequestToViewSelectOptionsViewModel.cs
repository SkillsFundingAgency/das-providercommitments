using System;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeshipTests;

public class WhenMappingSelectOptionsRequestToViewSelectOptionsViewModel
{
    private GetDraftApprenticeshipResponse _draftApprenticeshipApiResponse;
    private GetTrainingProgrammeResponse _standardOptionsResponse;
    private SelectOptionsRequest _selectOptionsRequest;
    private ViewStandardOptionsViewModelMapper _mapper;
    private Func<Task<ViewSelectOptionsViewModel>> _act;
    private Mock<ICommitmentsApiClient> _commitmentsApiClient;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _draftApprenticeshipApiResponse = fixture.Build<GetDraftApprenticeshipResponse>().Create();
        _standardOptionsResponse = fixture.Build<GetTrainingProgrammeResponse>().Create();
        _selectOptionsRequest = fixture.Build<SelectOptionsRequest>().Create();
            
        _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
        _commitmentsApiClient
            .Setup(x => x.GetTrainingProgrammeVersionByStandardUId(_draftApprenticeshipApiResponse.StandardUId, CancellationToken.None))
            .ReturnsAsync(_standardOptionsResponse);
        _commitmentsApiClient
            .Setup(x => x.GetDraftApprenticeship(_selectOptionsRequest.CohortId, _selectOptionsRequest.DraftApprenticeshipId, CancellationToken.None))
            .ReturnsAsync(_draftApprenticeshipApiResponse);

        _mapper = new ViewStandardOptionsViewModelMapper(_commitmentsApiClient.Object);

        _act = async () => await _mapper.Map(TestHelper.Clone(_selectOptionsRequest));
    }

    [Test]
    public async Task Then_The_Options_Are_Mapped()
    {
        var result = await _act();

        result.Options.Should().BeEquivalentTo(_standardOptionsResponse.TrainingProgramme.Options.ToList());
    }

    [Test]
    public async Task Then_The_DraftApprenticeshipId_Is_Mapped()
    {
        var result = await _act();

        result.DraftApprenticeshipId.Should().Be(_selectOptionsRequest.DraftApprenticeshipId);
    }

    [Test]
    public async Task Then_The_DraftApprenticeshipHashedId_Is_Mapped()
    {
        var result = await _act();

        result.DraftApprenticeshipHashedId.Should().Be(_selectOptionsRequest.DraftApprenticeshipHashedId);
    }
        
    [Test]
    public async Task Then_The_Cohort_Reference_Is_Mapped()
    {
        var result = await _act();

        result.CohortReference.Should().Be(_selectOptionsRequest.CohortReference);
    }

    [Test]
    public async Task Then_The_Cohort_Id_Is_Mapped()
    {
        var result = await _act();

        result.CohortId.Should().Be(_selectOptionsRequest.CohortId);
    }
        
    [Test]
    public async Task Then_The_Provider_Id_Is_Mapped()
    {
        var result = await _act();

        result.ProviderId.Should().Be(_selectOptionsRequest.ProviderId);
    }

    [Test]
    public async Task Then_The_TrainingCourseName_Is_Mapped()
    {
        var result = await _act();

        result.TrainingCourseName.Should().Be(_draftApprenticeshipApiResponse.TrainingCourseName);
    }

    [Test]
    public async Task Then_The_Standard_Version_Is_Mapped()
    {
        var result = await _act();

        result.TrainingCourseVersion.Should().Be(_draftApprenticeshipApiResponse.TrainingCourseVersion);
    }
        
    [Test]
    public async Task Then_The_Standard_IFate_Link_Is_Mapped()
    {
        var result = await _act();

        result.StandardPageUrl.Should().Be(_standardOptionsResponse.TrainingProgramme.StandardPageUrl);
    }

    [Test]
    public async Task Then_If_No_Options_Empty_List_Returned()
    {
        _standardOptionsResponse.TrainingProgramme.Options = null;
        _commitmentsApiClient
            .Setup(x => x.GetTrainingProgrammeVersionByStandardUId(_draftApprenticeshipApiResponse.StandardUId, CancellationToken.None))
            .ReturnsAsync(_standardOptionsResponse);
            
        var result = await _act();

        result.Options.Should().BeEmpty();
    }

    [Test]
    public async Task Then_The_Selected_Option_Is_Mapped()
    {
        var result = await _act();

        result.SelectedOption.Should().Be(_draftApprenticeshipApiResponse.TrainingCourseOption);
    }

    [Test]
    public async Task Then_If_The_Selected_Option_Is_Empty_Then_Set_To_Minus_One()
    {
        _draftApprenticeshipApiResponse.TrainingCourseOption = "";
                
        var result = await _act();

        result.SelectedOption.Should().Be("-1");
    }
        
    [Test]
    public async Task Then_If_The_Selected_Option_Is_Null_Then_Set_To_Null()
    {
        _draftApprenticeshipApiResponse.TrainingCourseOption = null;
                
        var result = await _act();

        result.SelectedOption.Should().BeNull();
    }
}