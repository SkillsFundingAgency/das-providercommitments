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
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Interfaces;

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
        public async Task GettingDeliveryModel_ForProviderAndCourse_WithMultipleOptions_ShouldRedirectToAddDraftApprenticeship()
        {
            var fixture = new WhenSelectingDeliveryModelFixture()
                .WithDeliveryModels(new List<DeliveryModel> { DeliveryModel.Regular, DeliveryModel.PortableFlexiJob });

            var result = await fixture.Sut.SelectDeliveryModel(fixture.Request) as ViewResult;
            result.ViewName.Should().Be("SelectDeliveryModel");
        }

        [Test]
        public async Task WhenSettingDeliveryModel_AndNoOptionSet_ShouldThrowException()
        {
            var fixture = new WhenSelectingDeliveryModelFixture();
            fixture.ViewModel.DeliveryModel = null;

            try
            {
                var result = await fixture.Sut.SetDeliveryModel(fixture.ViewModel);
                Assert.Fail("Should have had exception thrown");
            }
            catch (CommitmentsApiModelException e)
            {
                e.Errors[0].Field.Should().Be("DeliveryModel");
                e.Errors[0].Message.Should().Be("You must select the apprenticeship delivery model");
            }
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
        public SelectDeliveryModelViewModel ViewModel;

        public WhenSelectingDeliveryModelFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Build<CreateCohortWithDraftApprenticeshipRequest>().Create();
            ModelMapperMock = new Mock<IModelMapper>();
            ViewModel = fixture.Create<SelectDeliveryModelViewModel>();


            Sut = new CohortController(Mock.Of<IMediator>(), ModelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>());
        }

        public WhenSelectingDeliveryModelFixture WithDeliveryModels(List<DeliveryModel> list)
        {
            ModelMapperMock.Setup(x => x.Map<SelectDeliveryModelViewModel>(Request))
                .ReturnsAsync(new SelectDeliveryModelViewModel { DeliveryModels = list.ToArray() });
            return this;
        }

        public void VerifyReturnsRedirect(IActionResult redirectResult)
        {
            redirectResult.VerifyReturnsRedirect().Url.Equals(RedirectUrl);
        }
    }
}
