using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class OverlapOptionsForChangeEmployerRequestMapperTests
{
    private OverlapOptionsForChangeEmployerRequestMapper _mapper;
    private ChangeOfEmployerOverlapAlertViewModel _viewModel;

    [SetUp]
    public void SetUp()
    {
        var fixture = new Fixture();

        _viewModel = fixture.Build<ChangeOfEmployerOverlapAlertViewModel>()
            .Create();

        _mapper = new OverlapOptionsForChangeEmployerRequestMapper();
    }

    [Test, MoqAutoData]
    public async Task ApprenticeshipId_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);
        result.ApprenticeshipId.Should().Be(_viewModel.ApprenticeshipId);
    }

    [Test, MoqAutoData]
    public async Task ApprenticeshipHashedId_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.ApprenticeshipHashedId.Should().Be(_viewModel.ApprenticeshipHashedId);
    }

    [Test, MoqAutoData]
    public async Task ProviderId_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.ProviderId.Should().Be(_viewModel.ProviderId);
    }

    [Test, MoqAutoData]
    public async Task CacheKey_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.CacheKey.Should().Be(_viewModel.CacheKey);
    }
    
    [Test, MoqAutoData]
    public async Task ApprenticeshipStatus_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.Status.Should().Be(_viewModel.Status);
    }
}