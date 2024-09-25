using System;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class ValidateChangeOfEmployerOverlapApimRequestMapperTests
{
    private ValidateChangeOfEmployerOverlapApimRequestMapper _mapper;
    private TrainingDatesViewModel _viewModel;

    [SetUp]
    public void SetUp()
    {
        var baseDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var startDate = baseDate;
        var endDate = baseDate.AddYears(2);

        var fixture = new Fixture();

        _viewModel = fixture.Build<TrainingDatesViewModel>()
            //.With(x => x.DetailsAcknowledgement, true)
            .With(x => x.StartDate, new MonthYearModel(startDate.ToString("MMyyyy")))
            .With(x => x.EndDate, new MonthYearModel(endDate.ToString("MMyyyy")))
            .With(x => x.EmploymentEndDate, new MonthYearModel(endDate.ToString("MMyyyy")))
            .Create();

        _mapper = new ValidateChangeOfEmployerOverlapApimRequestMapper();
    }

    [Test, MoqAutoData]
    public async Task Uln_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);
        result.Uln.Should().Be(_viewModel.Uln);
    }

    [Test, MoqAutoData]
    public async Task ProviderId_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.ProviderId.Should().Be(_viewModel.ProviderId);
    }

    [Test, MoqAutoData]
    public async Task StartDate_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.StartDate.Should().Be(_viewModel.StartDate.Date.Value.ToString("dd-MM-yyyy"));
    }

    [Test, MoqAutoData]
    public async Task EndDate_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.EndDate.Should().Be(_viewModel.EndDate.Date.Value.ToString("dd-MM-yyyy"));
    }
}