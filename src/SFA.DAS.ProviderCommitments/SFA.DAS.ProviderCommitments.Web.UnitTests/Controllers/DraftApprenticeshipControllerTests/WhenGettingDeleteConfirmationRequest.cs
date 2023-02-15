using AutoFixture;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenGettingDeleteConfirmationRequest
    {
        public DraftApprenticeshipController Sut { get; set; }
        private Mock<IModelMapper> _modelMapperMock;
        private Mock<IAuthorizationService> _providerFeatureToggle;
        private DeleteConfirmationViewModel _viewModel;
        private DeleteConfirmationRequest _request;

        [SetUp]
        public void Arrange()
        {
            var _autoFixture = new Fixture();
            _request = _autoFixture.Create<DeleteConfirmationRequest>();
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = _autoFixture.Create<DeleteConfirmationViewModel>();

            _modelMapperMock
              .Setup(x => x.Map<DeleteConfirmationViewModel>(_request))
              .ReturnsAsync(_viewModel);

            _providerFeatureToggle = new Mock<IAuthorizationService>();
            _providerFeatureToggle.Setup(x => x.IsAuthorized(It.IsAny<string>())).Returns(false);

            Sut = new DraftApprenticeshipController(
                Mock.Of<IMediator>(),
                Mock.Of<ICommitmentsApiClient>(),
                _modelMapperMock.Object,
                Mock.Of<IEncodingService>(),
                _providerFeatureToggle.Object,
                Mock.Of<IOuterApiService>());
        }

        [Test]
        public async Task Then_Call_ModelMapper()
        {
            //Act
            await Sut.DeleteConfirmation(_request);

            //Assert
            _modelMapperMock.Verify(x => x.Map<DeleteConfirmationViewModel>(_request));
        }

        [Test]
        public async Task Then_Returns_View()
        {
            //Act
            var result = await Sut.DeleteConfirmation(_request);

            //Assert
            result.VerifyReturnsViewModel().WithModel<DeleteConfirmationViewModel>();
        }
    }
}