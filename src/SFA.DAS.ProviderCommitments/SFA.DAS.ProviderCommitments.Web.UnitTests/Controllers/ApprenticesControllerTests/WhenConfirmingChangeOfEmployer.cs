using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using AutoFixture;
using SFA.DAS.CommitmentsV2.Shared.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenConfirmingChangeOfEmployer
    {
        [Test]
        public async Task GetThenCallsPriceViewModelMapper()
        {
            var fixture = new WhenConfirmingChangeOfEmployerFixture();

            await fixture.Sut.ConfirmChangeOfEmployer(fixture.ChangeOfEmployerRequest);

            fixture.VerifyChangeOfRequestViewMapperWasCalled();
        }

        [Test]
        public async Task GetThenReturnsView()
        {
            var fixture = new WhenConfirmingChangeOfEmployerFixture();

            var result = await fixture.Sut.ConfirmChangeOfEmployer(fixture.ChangeOfEmployerRequest) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(ChangeOfEmployerViewModel), result.Model.GetType());
        }
    }

    public class WhenConfirmingChangeOfEmployerFixture
    {
        public ApprenticeController Sut { get; set; }
        public ChangeOfEmployerRequest ChangeOfEmployerRequest { get; set; }
        public ChangeOfEmployerViewModel ChangeOfEmployerViewModel { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly Fixture _fixture;

        public WhenConfirmingChangeOfEmployerFixture()
        {
            _fixture = new Fixture();
            ChangeOfEmployerRequest = _fixture.Build<ChangeOfEmployerRequest>().With(x=>x.StartDate, "042020").Create();
            ChangeOfEmployerViewModel = _fixture.Build<ChangeOfEmployerViewModel>().With(x => x.NewStartDate, new MonthYearModel("042020")).Create();

            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(x => x.Map<ChangeOfEmployerViewModel>(It.IsAny<ChangeOfEmployerRequest>()))
                .ReturnsAsync(ChangeOfEmployerViewModel);

            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>());
        }

        public void VerifyChangeOfRequestViewMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ChangeOfEmployerViewModel>(ChangeOfEmployerRequest));
        }
    }
}
