using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests;

[TestFixture]
public class WhenGettingChangeEmployer
{
    [Test]
    public async Task ThenCallsModelMapper()
    {
        var fixture = new ChangeEmployerFixture();

        await fixture.Act();

        fixture.VerifyMapperWasCalled();
    }

    [Test]
    public async Task ThenReturnsView()
    {
        var fixture = new ChangeEmployerFixture();

        var result = await fixture.Act() as ViewResult;

        result.Should().NotBeNull();
        result.Model.GetType().Should().Be(typeof(ChangeEmployerViewModel));

        var model = result.Model.Should().BeOfType<ChangeEmployerViewModel>().Subject;
        model.ProviderId.Should().Be(fixture.ProviderId);
        model.CacheKey.Should().Be(fixture.CacheKey);
    }
}

public class ChangeEmployerFixture
{
    public CohortController Sut { get; set; }
    public long ProviderId => _providerId;
    public Guid CacheKey => _cacheKey;

    private readonly Mock<IModelMapper> _modelMapperMock;
    private readonly ChangeEmployerRequest _request;
    private readonly long _providerId;
    private readonly Guid _cacheKey;

    public ChangeEmployerFixture()
    {
        _providerId = 123;
        _cacheKey = Guid.NewGuid();
        _request = new ChangeEmployerRequest { ProviderId = _providerId, CacheKey = _cacheKey };
        _modelMapperMock = new Mock<IModelMapper>();
        var viewModel = new ChangeEmployerViewModel
        {
            ProviderId = _providerId,
            CacheKey = _cacheKey,
            AccountProviderLegalEntities = new List<AccountProviderLegalEntityViewModel>(),
            BackLink = "Test.com"
        };

        _modelMapperMock
            .Setup(x => x.Map<ChangeEmployerViewModel>(_request))
            .ReturnsAsync(viewModel);

        Sut = new CohortController(Mock.Of<IMediator>(), _modelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
            Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
    }

    public ChangeEmployerFixture WithModelStateErrors()
    {
        Sut.ControllerContext.ModelState.AddModelError("TestError","Test Error");
        return this;
    }

    public void VerifyMapperWasCalled()
    {
        _modelMapperMock.Verify(x => x.Map<ChangeEmployerViewModel>(_request));
    }

    public async Task<IActionResult> Act() => await Sut.ChangeEmployer(_request);
}