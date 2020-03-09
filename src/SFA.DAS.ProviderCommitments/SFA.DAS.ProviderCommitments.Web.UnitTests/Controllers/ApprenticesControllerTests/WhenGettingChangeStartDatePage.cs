using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingChangeStartDatePage
    {
        private GetChangeStartDateFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new GetChangeStartDateFixture();
        }

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

            result.VerifyReturnsViewModel().WithModel<ChangeStartDateViewModel>();
        }
    }

    internal class GetChangeStartDateFixture
    {
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly ApprenticeController _sut;
        private readonly ChangeStartDateRequest _request;
        private readonly ChangeStartDateViewModel _viewModel;

        public GetChangeStartDateFixture()
        {
            _request = new ChangeStartDateRequest
            {
                ProviderId = 2342,
                EmployerAccountLegalEntityPublicHashedId = "AB34CDS",
                ApprenticeshipHashedId = "KG34DF989"
            };
            _viewModel = new ChangeStartDateViewModel();
            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock
                .Setup(x => x.Map<ChangeStartDateViewModel>(_request))
                .ReturnsAsync(_viewModel);

            _sut = new ApprenticeController(_modelMapperMock.Object);
        }

        public Task<IActionResult> Act() => _sut.ChangeStartDate(_request);

        public void Verify_ModelMapperWasCalled(Times times) => _modelMapperMock.Verify(x => x.Map<ChangeStartDateViewModel>(_request), times);
    }
}