using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

[TestFixture]
public class WhenGettingChangeEmployer
{
    private GetChangeEmployerPageFixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new GetChangeEmployerPageFixture();
    }

    [TestCase(typeof(InformViewModel), RouteNames.ChangeEmployerInform)]
    [TestCase(typeof(ChangeEmployerRequestDetailsViewModel), RouteNames.ChangeEmployerDetails)]
    public async Task Then_Returns_Appropriate_Redirect(Type mapperResultType, string expectedViewName)
    {
        _fixture.WithMapperResult(mapperResultType);
        var result = await _fixture.Act();
        var redirectToRouteResult = result.VerifyReturnsRedirectToRouteResult();
        Assert.That(redirectToRouteResult.RouteName, Is.EqualTo(expectedViewName));
    }
}

internal class GetChangeEmployerPageFixture
{
    private readonly Mock<IModelMapper> _modelMapper;
    private readonly ChangeEmployerRequest _request;
    private readonly InformViewModel _informViewModel;
    private readonly ChangeEmployerRequestDetailsViewModel _changeEmployerRequestDetailsViewModel;
    private readonly ApprenticeController _sut;

    public GetChangeEmployerPageFixture()
    {
        const long providerId = 123;
        const long apprenticeshipId = 345;
        const string apprenticeshipHashedId = "DS23JF3";
        _request = new ChangeEmployerRequest
        {
            ProviderId = providerId,
            ApprenticeshipHashedId = apprenticeshipHashedId,
            ApprenticeshipId = apprenticeshipId
        };
        _informViewModel = new InformViewModel
        {
            ProviderId = providerId,
            ApprenticeshipHashedId = apprenticeshipHashedId,
            ApprenticeshipId = apprenticeshipId
        };
        _changeEmployerRequestDetailsViewModel = new ChangeEmployerRequestDetailsViewModel();

        _modelMapper = new Mock<IModelMapper>();
        _modelMapper
            .Setup(x => x.Map<IChangeEmployerViewModel>(_request))
            .ReturnsAsync(_informViewModel);

        var tempDataProvider = Mock.Of<ITempDataProvider>();
        var tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
        var tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());

        _sut = new ApprenticeController(_modelMapper.Object,
            Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(), 
            Mock.Of<ICommitmentsApiClient>(),
            Mock.Of<IOuterApiService>(),
            Mock.Of<ICacheStorageService>())
        {
            TempData = tempData
        };
    }

    public GetChangeEmployerPageFixture WithMapperResult(Type mapperResultType)
    {
        if (mapperResultType == typeof(InformViewModel))
        {
            _modelMapper
                .Setup(x => x.Map<IChangeEmployerViewModel>(_request))
                .ReturnsAsync(_informViewModel);
        }
        else
        {
            _modelMapper
                .Setup(x => x.Map<IChangeEmployerViewModel>(_request))
                .ReturnsAsync(_changeEmployerRequestDetailsViewModel);
        }

        return this;
    }

    public Task<IActionResult> Act() => _sut.ChangeEmployer(_request);
}