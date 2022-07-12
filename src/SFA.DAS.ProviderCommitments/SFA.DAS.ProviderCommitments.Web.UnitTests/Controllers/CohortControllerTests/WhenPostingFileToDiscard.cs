using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System;
using SFA.DAS.Authorization.Services;

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
            Assert.AreEqual("FileUploadReviewDelete", redirect.ActionName);
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
            Assert.AreEqual("FileUploadReview", redirect.ActionName);
        }
    }

    public class WhenPostingFileToDiscardFixture
    {
        private CohortController _sut { get; set; }
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly FileDiscardViewModel _viewModel;

        public WhenPostingFileToDiscardFixture(bool fileDiscardConfirmed)
        {
            var fixture = new Fixture();
            _viewModel = fixture.Create<FileDiscardViewModel>();
            _viewModel.FileDiscardConfirmed = fileDiscardConfirmed;

            _modelMapper = new Mock<IModelMapper>();

            _sut = new CohortController(Mock.Of<IMediator>(), _modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>(),  Mock.Of<IOuterApiService>());
        }

        public IActionResult Act() => _sut.FileUploadDiscard(_viewModel);
    }
}
