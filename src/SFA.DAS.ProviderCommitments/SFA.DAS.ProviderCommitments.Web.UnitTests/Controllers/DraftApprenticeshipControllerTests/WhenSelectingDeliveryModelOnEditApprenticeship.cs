using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenSelectingDeliveryModelOnEditApprenticeship
    {
        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithOnlyOneOption_ShouldRedirectToEditDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelOnEditApprenticeshipFixture()
                .WithDraftApprenticeship()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular });

            var result = await fixture.Sut.SelectDeliveryModelForEdit(fixture.Request) as RedirectToActionResult;
            result.ActionName.Should().Be("EditDraftApprenticeship");
        }

        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithMultipleOptions_ShouldRedirectToSelectDeliveryModel()
        {
            var fixture = new WhenSelectingDeliveryModelOnEditApprenticeshipFixture()
                .WithDraftApprenticeship()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob });

            var result = await fixture.Sut.SelectDeliveryModelForEdit(fixture.Request) as ViewResult;
            result.Should().NotBeNull();
        }

        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithOnlyOneInvalidOption_ShouldRedirectToSelectDeliveryModel()
        {
            var fixture = new WhenSelectingDeliveryModelOnEditApprenticeshipFixture()
                .WithDraftApprenticeship()
                .WithUnavailableDeliveryModel()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular });

            var result = await fixture.Sut.SelectDeliveryModelForEdit(fixture.Request) as ViewResult;
            result.Should().NotBeNull();
        }

        [Test]
        public void WhenSettingDeliveryModel_AndOptionSet_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelOnEditApprenticeshipFixture()
                .WithDraftApprenticeship()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob });

            fixture.ViewModel.DeliveryModel = DeliveryModel.PortableFlexiJob;

            var result = fixture.Sut.SetDeliveryModelForEdit(fixture.ViewModel) as RedirectToActionResult;
            result.ActionName.Should().Be("EditDraftApprenticeship");
        }
    }

    public class WhenSelectingDeliveryModelOnEditApprenticeshipFixture
    {
        private readonly Mock<ITempDataDictionary> _tempDataMock;
        private readonly EditDraftApprenticeshipViewModel _draftApprenticeship;
        private readonly SelectDeliveryModelForEditViewModel _mapperResult;
        
        public DraftApprenticeshipController Sut { get; }
        public SelectDeliveryModelForEditViewModel ViewModel { get; }
        public DraftApprenticeshipRequest Request { get; }
        
        public WhenSelectingDeliveryModelOnEditApprenticeshipFixture()
        {
            var fixture = new Fixture();
            ViewModel = fixture.Create<SelectDeliveryModelForEditViewModel>();
            Request = fixture.Create<DraftApprenticeshipRequest>();
            _draftApprenticeship = fixture.Build<EditDraftApprenticeshipViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x => x.StartDate)
                .Without(x => x.EndMonth).Without(x => x.EndYear)
                .Create();

            var modelMapperMock = new Mock<IModelMapper>();
            _tempDataMock = new Mock<ITempDataDictionary>();
            var authorizationServiceMock = new Mock<IAuthorizationService>();

            _mapperResult = new SelectDeliveryModelForEditViewModel();
            modelMapperMock.Setup(x => x.Map<SelectDeliveryModelForEditViewModel>(It.IsAny<DraftApprenticeshipRequest>()))
                .ReturnsAsync(_mapperResult);

            Sut = new DraftApprenticeshipController(
                Mock.Of<IMediator>(),
                Mock.Of<ICommitmentsApiClient>(),
                modelMapperMock.Object,
                Mock.Of<IEncodingService>(),
                authorizationServiceMock.Object, 
                Mock.Of<IOuterApiService>(),
                Mock.Of<IAuthenticationService>(),
                Mock.Of<ICacheStorageService>());
            
            Sut.TempData = _tempDataMock.Object;
        }

        public WhenSelectingDeliveryModelOnEditApprenticeshipFixture WithDeliveryModels(List<DeliveryModel> list)
        {
            _mapperResult.DeliveryModels = list;
            return this;
        }

        public WhenSelectingDeliveryModelOnEditApprenticeshipFixture WithDraftApprenticeship()
        {
            object asString = JsonConvert.SerializeObject(_draftApprenticeship);
            _tempDataMock.Setup(x => x.Peek(It.IsAny<string>())).Returns(asString);
            return this;
        }

        public WhenSelectingDeliveryModelOnEditApprenticeshipFixture WithUnavailableDeliveryModel()
        {
            _mapperResult.HasUnavailableFlexiJobAgencyDeliveryModel = true;
            return this;
        }
    }
}
