using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingDataLockConfirmRestart
    {
        private ApprenticeController _sut;
        private Mock<IModelMapper> _modelMapperMock;
        private DatalockConfirmRestartRequest _request;
        private DatalockConfirmRestartViewModel _viewModel;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Create<DatalockConfirmRestartRequest>();
            _viewModel = fixture.Create<DatalockConfirmRestartViewModel>();
            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(x => x.Map<DatalockConfirmRestartViewModel>(_request)).ReturnsAsync(_viewModel);
            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        [Test]
        public void ThenCallsModelMapper()
        {
            //Act
             _sut.ConfirmRestart(_request);

            //Assert
            _modelMapperMock.Verify(x => x.Map<DatalockConfirmRestartViewModel>(_request));
        }

        [Test]
        public void ThenReturnsView()
        {
            //Act
            var result =  _sut.ConfirmRestart(_request) as ViewResult;

            //Assert
            Assert.NotNull(result);
            Assert.AreEqual(typeof(DatalockConfirmRestartViewModel), result.Model.GetType());
        }
    }
}
