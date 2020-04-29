using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;

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

        [TestCase(typeof(InformViewModel), "Inform")]
        [TestCase(typeof(ChangeEmployerRequestDetailsViewModel), "ChangeEmployerRequestDetails")]
        public async Task Then_Returns_Appropriate_View(Type mapperResultType, string expectedViewName)
        {
            _fixture.WithMapperResult(mapperResultType);
            var result = await _fixture.Act();
            var view = result.VerifyReturnsViewModel();
            Assert.AreEqual(expectedViewName, view.ViewName);
            Assert.AreEqual(mapperResultType, view.Model.GetType());
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
            _sut = new ApprenticeController(_modelMapper.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
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