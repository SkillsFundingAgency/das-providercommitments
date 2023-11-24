using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingConfirmDataLockChanges
    {
        private ApprenticeController _sut;
        private Mock<IModelMapper> _modelMapperMock;
        private ConfirmDataLockChangesRequest _request;
        private ConfirmDataLockChangesViewModel _viewModel;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Create<ConfirmDataLockChangesRequest>();
            _viewModel = fixture.Create<ConfirmDataLockChangesViewModel>();
            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(x => x.Map<ConfirmDataLockChangesViewModel>(_request)).ReturnsAsync(_viewModel);
            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>());
        }

        [Test]
        public async Task ThenCallsModelMapper()
        {
            //Act
            await _sut.ConfirmDataLockChanges(_request);

            //Assert
            _modelMapperMock.Verify(x => x.Map<ConfirmDataLockChangesViewModel>(_request));
        }

        [Test]
        public async Task ThenReturnsView()
        {
            //Act
            var result = await _sut.ConfirmDataLockChanges(_request) as ViewResult;

            //Assert
            Assert.NotNull(result);
            Assert.AreEqual(typeof(ConfirmDataLockChangesViewModel), result.Model.GetType());
        }
    }
}
