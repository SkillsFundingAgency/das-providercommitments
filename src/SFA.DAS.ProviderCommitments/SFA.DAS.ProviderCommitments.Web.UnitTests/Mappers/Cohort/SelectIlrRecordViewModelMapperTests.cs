﻿using System.Linq;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class SelectIlrRecordViewModelMapperTests
{
    private SelectIlrRecordViewModelMapper _mapper;
    private Mock<IOuterApiService> _outerApiService;
    private SelectIlrRecordRequest _request;
    private GetIlrDetailsForProviderResponse _apiResponse;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _request = fixture.Create<SelectIlrRecordRequest>();
        _apiResponse = fixture.Create<GetIlrDetailsForProviderResponse>();

        _outerApiService = new Mock<IOuterApiService>();

        _outerApiService.Setup(x => x.GetIlrDetailsForProvider(_request.ProviderId, _request.AccountLegalEntityId, _request.SearchTerm, _request.SortField, _request.ReverseSort, 1))
            .ReturnsAsync(_apiResponse);

        _mapper = new SelectIlrRecordViewModelMapper(_outerApiService.Object);
    }

    [Test]
    public async Task MapToFilterModelCorrectly()
    {
        var result = await _mapper.Map(_request);
        result.FilterModel.ProviderId.Should().Be(_request.ProviderId);
        result.FilterModel.EmployerAccountLegalEntityPublicHashedId.Should().Be(_request.EmployerAccountLegalEntityPublicHashedId);
        result.FilterModel.TotalNumberOfApprenticeshipsFound.Should().Be(_apiResponse.Total);
        result.FilterModel.PageNumber.Should().Be(_apiResponse.Page);
        result.FilterModel.SortField.Should().Be(_request.SortField);
        result.FilterModel.ReverseSort.Should().Be(_request.ReverseSort);
        result.FilterModel.SearchTerm.Should().Be(_request.SearchTerm);
    }

    [Test]
    public async Task MapToViewModelCorrectly()
    {
        var result = await _mapper.Map(_request);
        result.ProviderId.Should().Be(_request.ProviderId);
        result.EmployerAccountLegalEntityPublicHashedId.Should().Be(_request.EmployerAccountLegalEntityPublicHashedId);
        result.EmployerAccountName.Should().Be(_apiResponse.EmployerName);
    }

    [Test]
    public async Task MapLearnersCorrectly()
    {
        var result = await _mapper.Map(_request);
        result.IlrApprenticeships.Should().BeEquivalentTo(_apiResponse.Learners.Select(x=>(IlrApprenticeshipSummary)x).ToList());
    }
}