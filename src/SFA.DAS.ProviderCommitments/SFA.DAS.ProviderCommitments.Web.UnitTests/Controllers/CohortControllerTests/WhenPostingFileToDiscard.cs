using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenPostingFileToDiscard
    {
        [Test]
        public void And_User_Selected_To_DiscardFile_Then_Redirect_To_FileDiscardSuccess_Page()
        {
            //Arrange
            var fixture = new WhenPostingFileToDiscardFixture(true);            

            //Act
            var result = fixture.Act();

            //Assert           
            var redirect = result.VerifyReturnsRedirectToActionResult();
            Assert.That(redirect.ActionName, Is.EqualTo("FileUploadReviewDelete"));
        }


        [Test]
        public void And_User_Selected_Not_To_DiscardFile_Then_Redirect_To_FileUploadCheck_Page()
        {
            //Arrange
            var fixture = new WhenPostingFileToDiscardFixture(false);

            //Act
            var result = fixture.Act();

            //Assert           
            var redirect = result.VerifyReturnsRedirectToActionResult();
            Assert.That(redirect.ActionName, Is.EqualTo("FileUploadReview"));
        }
    }

    public class WhenPostingFileToDiscardFixture
    {
        private readonly CohortController _sut;
        private readonly FileDiscardViewModel _viewModel;

        public WhenPostingFileToDiscardFixture(bool fileDiscardConfirmed)
        {
            var fixture = new Fixture();
            _viewModel = fixture.Create<FileDiscardViewModel>();
            _viewModel.FileDiscardConfirmed = fileDiscardConfirmed;

            var modelMapper = new Mock<IModelMapper>();

            _sut = new CohortController(Mock.Of<IMediator>(), modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IEncodingService>(),  Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public IActionResult Act() => _sut.FileUploadDiscard(_viewModel);
    }
}
