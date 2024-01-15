using AutoFixture;
using Microsoft.AspNetCore.Mvc;
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
    public class WhenGettingDataLockConfirmRestart
    {
        private ApprenticeController _sut;
        private DatalockConfirmRestartRequest _request;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Create<DatalockConfirmRestartRequest>();
            _sut = new ApprenticeController(Mock.Of<IModelMapper>(), Mock.Of<ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>());
        }

        [Test]
        public void ThenReturnsView()
        {
            //Act
            var result = _sut.ConfirmRestart(_request) as ViewResult;

            //Assert
            Assert.NotNull(result);
            Assert.AreEqual(typeof(DatalockConfirmRestartViewModel), result.Model.GetType());
        }
    }
}
