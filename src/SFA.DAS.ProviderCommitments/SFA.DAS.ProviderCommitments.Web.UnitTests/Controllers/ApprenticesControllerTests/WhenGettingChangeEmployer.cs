using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
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
            Assert.AreEqual(expectedViewName, redirectToRouteResult.RouteName);
        }
    }

    internal class GetChangeEmployerPageFixture
    {
        private string _apprenticeshipHashedId;
        private long _apprenticeshipId;
        private Mock<IModelMapper> _modelMapper;
        private long _providerId;
        private ChangeEmployerRequest _request;
        private InformViewModel _informViewModel;
        private ChangeEmployerRequestDetailsViewModel _changeEmployerRequestDetailsViewModel;
        private ApprenticeController _sut;

        public GetChangeEmployerPageFixture()
        {
            _providerId = 123;
            _apprenticeshipId = 345;
            _apprenticeshipHashedId = "DS23JF3";
            _request = new ChangeEmployerRequest
            {
                ProviderId = _providerId,
                ApprenticeshipHashedId = _apprenticeshipHashedId,
                ApprenticeshipId = _apprenticeshipId
            };
            _informViewModel = new InformViewModel
            {
                ProviderId = _providerId,
                ApprenticeshipHashedId = _apprenticeshipHashedId,
                ApprenticeshipId = _apprenticeshipId
            };
            _changeEmployerRequestDetailsViewModel = new ChangeEmployerRequestDetailsViewModel();

            _modelMapper = new Mock<IModelMapper>();
            _modelMapper
                .Setup(x => x.Map<IChangeEmployerViewModel>(_request))
                .ReturnsAsync(_informViewModel);

            ITempDataProvider tempDataProvider = Mock.Of<ITempDataProvider>();
            TempDataDictionaryFactory tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
            ITempDataDictionary tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());

            _sut = new ApprenticeController(_modelMapper.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>())
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
}