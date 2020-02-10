using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class WhenGettingInformPage
    {
        private GetInformPageFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new GetInformPageFixture();
        }

        [Test]
        public async Task ThenCallsModelMapper()
        {
            await _fixture.Act();

            _fixture.Verify_IModelMapper_IsCalled_Once();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var result = await _fixture.Act();

            result.VerifyReturnsViewModel().WithModel<InformViewModel>();
        }
    }

    internal class GetInformPageFixture
    {
        private string _apprenticeshipHashedId;
        private long _apprenticeshipId;
        private Mock<IModelMapper> _modelMapper;
        private long _providerId;
        private InformRequest _request;
        private InformViewModel _viewModel;
        private ApprenticeController _sut;

        public GetInformPageFixture()
        {
            _providerId = 123;
            _apprenticeshipId = 345;
            _apprenticeshipHashedId = "DS23JF3";
            _request = new InformRequest
            {
                ProviderId = _providerId,
                ApprenticeshipHashedId = _apprenticeshipHashedId,
                ApprenticeshipId = _apprenticeshipId
            };
            _viewModel = new InformViewModel
            {
                ProviderId = _providerId,
                ApprenticeshipHashedId = _apprenticeshipHashedId,
                ApprenticeshipId = _apprenticeshipId
            };
            _modelMapper = new Mock<IModelMapper>();
            _modelMapper
                .Setup(x => x.Map<InformViewModel>(_request))
                .ReturnsAsync(_viewModel);
            _sut = new ApprenticeController(_modelMapper.Object);
        }

        public async Task<IActionResult> Act() => await _sut.Inform(_request);

        public void Verify_IModelMapper_IsCalled_Once() => _modelMapper.Verify(x => x.Map<InformViewModel>(_request), Times.Once);
    }
}