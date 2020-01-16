using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.UnitTests.Customisations;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Filters
{
    public class WhenAddingGoogleAnalyticsInformation
    {
        [Test, MoqAutoData]
        public async Task ThenProviderIdIsAddedToViewBag(
            uint ukPrn,
            [ArrangeActionContext] ActionExecutingContext context,
            [Frozen] Mock<ActionExecutionDelegate> nextMethod,
            GoogleAnalyticsFilter filter)
        {
            //Arrange
            var claim = new Claim(ProviderClaims.Ukprn, ukPrn.ToString());
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { claim }));

            //Act
            await filter.OnActionExecutionAsync(context, nextMethod.Object);

            //Assert
            var actualController = context.Controller as Controller;
            Assert.IsNotNull(actualController);
            var viewBagData = actualController.ViewBag.GaData as GaData;
            Assert.IsNotNull(viewBagData);
            Assert.AreEqual(ukPrn.ToString(), viewBagData.UkPrn);
        }

        [Test, MoqAutoData]
        public async Task ThenIfNotAControllerGaDataNotPopu(
            long ukPrn,
            [ArrangeActionContext] ActionExecutingContext context,
            [Frozen] Mock<ActionExecutionDelegate> nextMethod,
            GoogleAnalyticsFilter filter)
        {
            //Arrange
            var claim = new Claim(ProviderClaims.Ukprn, ukPrn.ToString());
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { claim }));

            var contextWithoutController = new ActionExecutingContext(
                new ActionContext(context.HttpContext, context.RouteData, context.ActionDescriptor),
                context.Filters,
                context.ActionArguments,
                "");

            //Act
            await filter.OnActionExecutionAsync(contextWithoutController, nextMethod.Object);

            //Assert
            Assert.DoesNotThrowAsync(() => filter.OnActionExecutionAsync(contextWithoutController, nextMethod.Object));
        }
    }
}