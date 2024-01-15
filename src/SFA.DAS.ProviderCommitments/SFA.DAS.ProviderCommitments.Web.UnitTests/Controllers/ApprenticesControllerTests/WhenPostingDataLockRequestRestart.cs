using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenPostingDataLockRequestRestart
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
            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>());
        }

        [Test]
        public void Then_Redirect_To_Details_Page()
        {
            //Act
            var result = _sut.DataLockRequestRestart(_viewModel);

            //Assert
            result.VerifyReturnsRedirectToActionResult().WithActionName("Details");
        }


        [Test]
        public void Then_Redirect_To_ConfirmRestart_Page()
        {
            //Arrange
            _viewModel.SubmitStatusViewModel = SubmitStatusViewModel.Confirm;

            //Act
            var result = _sut.DataLockRequestRestart(_viewModel);

            //Assert
            result.VerifyReturnsRedirectToActionResult().WithActionName("ConfirmRestart");
        }
    }
}
