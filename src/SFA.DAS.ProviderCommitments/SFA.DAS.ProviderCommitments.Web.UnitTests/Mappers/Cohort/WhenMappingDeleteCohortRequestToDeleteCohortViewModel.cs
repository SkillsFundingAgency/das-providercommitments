using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenMappingDeleteCohortRequestToDeleteCohortViewModel
{
    [Test]
    public async Task Then_TheCohortReferenceIsMapped()
    {
        var fixture = new WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_CohortReference_Is_Mapped();
    }

    [Test]
    public async Task Then_EmployerNameIsMapped()
    {
        var fixture = new WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_EmployerName_Is_Mapped();
    }

    [Test]
    public async Task Then_NumberOfApprenticesAreMapped()
    {
        var fixture = new WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_NumberOfApprentices_Are_Mapped();
    }

    [Test]
    public async Task Then_ApprenticeshipTrainingProgrammeAreMappedCorrectly()
    {
        var fixture = new WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_ApprenticeshipTrainingProgrammeAreMappedCorrectly();
    }

    [Test]
    public async Task Then_ProviderId_IsMapped()
    {
        var fixture = new WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_ProviderId_IsMapped();
    }
}

public class WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture
{
    private readonly DeleteCohortRequest _deleteCohortRequest;
    private DeleteCohortViewModel _deleteCohortViewModel;
    private readonly DeleteCohortRequestViewModelMapper _mapper;
    private readonly GetCohortResponse _getCohortResponse;
    private readonly GetDraftApprenticeshipsResponse _getDraftApprenticeshipsResponse;

    private const long ProviderId = 1;
    private const long CohortId = 22;

    public WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture()
    {
        _deleteCohortRequest = new DeleteCohortRequest
            { ProviderId = ProviderId, CohortId = CohortId, CohortReference = "XYZ" };
        var commitmentsApiClient = new Mock<ICommitmentsApiClient>();
        _getCohortResponse = CreateGetCohortResponse();
        _getDraftApprenticeshipsResponse = CreateGetDraftApprenticeships();
        commitmentsApiClient.Setup(c => c.GetCohort(CohortId, CancellationToken.None))
            .ReturnsAsync(_getCohortResponse);
        commitmentsApiClient.Setup(c => c.GetDraftApprenticeships(CohortId, CancellationToken.None))
            .ReturnsAsync(_getDraftApprenticeshipsResponse);

        _mapper = new DeleteCohortRequestViewModelMapper(commitmentsApiClient.Object);
    }

    private static GetDraftApprenticeshipsResponse CreateGetDraftApprenticeships()
    {
        return new GetDraftApprenticeshipsResponse
        {
            DraftApprenticeships = new List<DraftApprenticeshipDto>
            {
                new()
                {
                    Id = 1,
                    CourseName = "Course1"
                },
                new()
                {
                    Id = 2,
                    CourseName = "Course1"
                },
                new()
                {
                    Id = 3,
                    CourseName = "Course2"
                }
            }
        };
    }

    public async Task<WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture> Map()
    {
        _deleteCohortViewModel = await _mapper.Map(_deleteCohortRequest);
        return this;
    }

    private static GetCohortResponse CreateGetCohortResponse()
    {
        return new GetCohortResponse
        {
            CohortId = 2,
            AccountLegalEntityId = 3,
            ProviderName = "asdf",
            LegalEntityName = "Employer1",
            WithParty = Party.Provider,
        };
    }

    internal void Verify_CohortReference_Is_Mapped()
    {
        _deleteCohortViewModel.CohortReference.Should().Be(_deleteCohortRequest.CohortReference);
    }

    internal void Verify_EmployerName_Is_Mapped()
    {
        _deleteCohortViewModel.EmployerAccountName.Should().Be(_getCohortResponse.LegalEntityName);
    }

    internal void Verify_NumberOfApprentices_Are_Mapped()
    {
        _deleteCohortViewModel.NumberOfApprenticeships.Should().Be(_getDraftApprenticeshipsResponse.DraftApprenticeships.Count);
    }

    internal void Verify_ApprenticeshipTrainingProgrammeAreMappedCorrectly()
    {
        Assert.Multiple(() =>
        {
            _deleteCohortViewModel.ApprenticeshipTrainingProgrammes.Any(x => x == "2 Course1").Should().BeTrue();
            _deleteCohortViewModel.ApprenticeshipTrainingProgrammes.Count.Should().Be(2);
            _deleteCohortViewModel.ApprenticeshipTrainingProgrammes.Any(x => x == "1 Course2").Should().BeTrue();
        });
    }

    internal void Verify_ProviderId_IsMapped()
    {
        _deleteCohortViewModel.ProviderId.Should().Be(_deleteCohortRequest.ProviderId);
    }
}