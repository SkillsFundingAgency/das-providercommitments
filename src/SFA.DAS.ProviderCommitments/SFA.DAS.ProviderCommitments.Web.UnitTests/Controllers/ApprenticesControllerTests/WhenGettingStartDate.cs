using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingStartDate
    {
        private GetStartDateFixture _fixture;

        [Test]
        public async Task ThenCallsModelMapper()
        {
            await _fixture.Act();

            _fixture.Verify_ModelMapperWasCalled(Times.Once());
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var result = await _fixture.Act();

            result.VerifyReturnsViewModel().WithModel<StartDateViewModel>();
        }

        [SetUp]
        public void SetUp()
        {
            _fixture = new GetStartDateFixture();
        }
    }

    internal class GetStartDateFixture
    {
        private readonly Mock<ICookieStorageService<IndexRequest>> _cookieStorageServiceMock;
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly StartDateRequest _request;
        private readonly ApprenticeController _sut;
        private readonly StartDateViewModel _viewModel;

        public GetStartDateFixture()
        {
            _request = new StartDateRequest
            {
                ProviderId = 2342,
                EmployerAccountLegalEntityPublicHashedId = "AB34CDS",
                ApprenticeshipHashedId = "KG34DF989"
            };
            _viewModel = new StartDateViewModel();
            _cookieStorageServiceMock = new Mock<ICookieStorageService<IndexRequest>>();
            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock
                .Setup(x => x.Map<StartDateViewModel>(_request))
                .ReturnsAsync(_viewModel);

            _sut = new ApprenticeController(_modelMapperMock.Object, _cookieStorageServiceMock.Object, Mock.Of<ICommitmentsApiClient>());
        }

        public Task<IActionResult> Act() => _sut.StartDate(_request);

        public void Verify_ModelMapperWasCalled(Times times) => _modelMapperMock.Verify(x => x.Map<StartDateViewModel>(_request), times);
    }
}