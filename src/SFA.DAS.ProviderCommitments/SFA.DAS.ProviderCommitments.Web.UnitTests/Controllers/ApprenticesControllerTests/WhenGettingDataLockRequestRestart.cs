using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingDataLockRequestRestart
    {        
        private ApprenticeController _sut;
        private Mock<IModelMapper> _modelMapperMock;
        private DataLockRequestRestartRequest _request;
        private DataLockRequestRestartViewModel _viewModel;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Create<DataLockRequestRestartRequest>();
            _viewModel = fixture.Create<DataLockRequestRestartViewModel>();
            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(x => x.Map<DataLockRequestRestartViewModel>(_request)).ReturnsAsync(_viewModel);
            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        [Test]
        public async Task ThenCallsModelMapper()
        {
            //Act
            await _sut.DataLockRequestRestart(_request);

            //Assert
            _modelMapperMock.Verify(x => x.Map<DataLockRequestRestartViewModel>(_request));
        }

        [Test]
        public async Task ThenReturnsView()
        {        
            //Act
            var result = await _sut.DataLockRequestRestart(_request) as ViewResult;

            //Assert
            Assert.NotNull(result);
            Assert.AreEqual(typeof(DataLockRequestRestartViewModel), result.Model.GetType());
        }

    }
}
