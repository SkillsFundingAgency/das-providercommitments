using System.Collections.Generic;
using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenSelectingDeliveryModel
    {
        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithOnlyOneOption_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelFixture()
                .WithDeliveryModels(new List<DeliveryModel> {DeliveryModel.Regular});

            var result = await fixture.Sut.SelectDeliveryModel(fixture.Request) as RedirectToActionResult;
            result.ActionName.Should().Be("AddDraftApprenticeship");
        }

        [Test]
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithMultipleOptions_ShouldRedirectTo_SelectDeliveryModel()
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
        public CohortController Sut { get; set; }

        public string RedirectUrl;
        public Mock<IModelMapper> ModelMapperMock;
        public CreateCohortWithDraftApprenticeshipRequest Request;
        public Web.Models.Cohort.SelectDeliveryModelViewModel ViewModel;

        public WhenSelectingDeliveryModelFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Build<CreateCohortWithDraftApprenticeshipRequest>().Create();
            ModelMapperMock = new Mock<IModelMapper>();
            ViewModel = fixture.Create<Web.Models.Cohort.SelectDeliveryModelViewModel>();

            ModelMapperMock.Setup(x => x.Map<CreateCohortWithDraftApprenticeshipRequest>(ViewModel)).ReturnsAsync(Request);

            Sut = new CohortController(Mock.Of<IMediator>(), ModelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(), Mock.Of<IAuthenticationService>());
        }

        public WhenSelectingDeliveryModelFixture WithDeliveryModels(List<DeliveryModel> list)
        {
            ModelMapperMock.Setup(x => x.Map<Web.Models.Cohort.SelectDeliveryModelViewModel>(Request))
                .ReturnsAsync(new Web.Models.Cohort.SelectDeliveryModelViewModel { DeliveryModels = list });
            return this;
        }

        public void VerifyReturnsRedirect(IActionResult redirectResult)
        {
            redirectResult.VerifyReturnsRedirect().Url.Equals(RedirectUrl);
        }
    }
}
