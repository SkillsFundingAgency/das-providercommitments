using System.Collections.Generic;
using FluentAssertions;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderUrlHelper;
using SelectDeliveryModelViewModel = SFA.DAS.ProviderCommitments.Web.Models.Cohort.SelectDeliveryModelViewModel;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenSelectingDeliveryModel
    {
        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithOnlyOneOption_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular });

            var result = await fixture.Sut.SelectDeliveryModel(fixture.Request) as RedirectToActionResult;
            result.ActionName.Should().Be("AddDraftApprenticeship");
        }

        [Test]
        public async Task
            GettingDeliveryModel_ForProviderAndCourse_WithMultipleOptions_ShouldRedirectTo_SelectDeliveryModel()
        {
            var fixture = new WhenSelectingDeliveryModelFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob });

            var result = await fixture.Sut.SelectDeliveryModel(fixture.Request) as ViewResult;
            result.ViewName.Should().Be(null);
        }

        [Test]
        public async Task WhenSettingDeliveryModel_AndOptionSet_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelFixture();

            fixture.ViewModel.DeliveryModel = DeliveryModel.PortableFlexiJob;

            var result = await fixture.Sut.SetDeliveryModel(fixture.ViewModel) as RedirectToActionResult;
            result.ActionName.Should().Be("AddDraftApprenticeship");
        }
    }

    public class WhenSelectingDeliveryModelFixture
    {
        private readonly Mock<IModelMapper> _modelMapperMock;
        public CreateCohortWithDraftApprenticeshipRequest Request { get; }
        public SelectDeliveryModelViewModel ViewModel { get; }
        public CohortController Sut { get; }

        public WhenSelectingDeliveryModelFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Build<CreateCohortWithDraftApprenticeshipRequest>().Create();
            _modelMapperMock = new Mock<IModelMapper>();
            ViewModel = fixture.Create<SelectDeliveryModelViewModel>();

            _modelMapperMock.Setup(x => x.Map<CreateCohortWithDraftApprenticeshipRequest>(ViewModel))
                .ReturnsAsync(Request);

            Sut = new CohortController(Mock.Of<IMediator>(), _modelMapperMock.Object, Mock.Of<ILinkGenerator>(),
                Mock.Of<ICommitmentsApiClient>(),
                Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(), Mock.Of<IAuthorizationService>());
        }

        public WhenSelectingDeliveryModelFixture WithDeliveryModels(List<DeliveryModel> list)
        {
            _modelMapperMock.Setup(x => x.Map<SelectDeliveryModelViewModel>(Request))
                .ReturnsAsync(new SelectDeliveryModelViewModel { DeliveryModels = list });
            return this;
        }
    }
}