using System.Security.Claims;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.ProviderCommitments.Web.UnitTests.Customisations;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Filters
{
    public class WhenAddingGoogleAnalyticsInformation
    {
        [Test, DomainAutoData]
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
            Assert.That(actualController, Is.Not.Null);
            var viewBagData = actualController.ViewBag.GaData as GaData;
            Assert.That(viewBagData, Is.Not.Null);
            Assert.That(viewBagData.UkPrn, Is.EqualTo(ukPrn.ToString()));
        }

        [Test, DomainAutoData]
        public async Task AndContextIsNonController_ThenNoDataIsAddedToViewbag(
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

