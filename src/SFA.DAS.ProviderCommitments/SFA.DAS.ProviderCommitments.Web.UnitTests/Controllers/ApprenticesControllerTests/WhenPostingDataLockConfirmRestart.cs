using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenPostingDataLockConfirmRestart
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
        public void Then_Redirect_To_Details_Page()
        {
            //Act
            var result = _sut.ConfirmRestart(_viewModel);

            //Assert
            result.VerifyReturnsRedirectToActionResult().WithActionName("Details");
        }

        [Test]
        public void Then_Redirect_To_ConfirmRestart_Page()
        {
            //Arrange
            _viewModel.SendRequestToEmployer = true;

            //Act
            var result = _sut.ConfirmRestart(_viewModel);

            //Assert
            //Verify the api been called
        }
    }
}
