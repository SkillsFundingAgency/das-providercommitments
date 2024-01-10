namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers
{
    internal static class IActionResultTestExtensions
    {
        public static ViewResult VerifyReturnsViewModel(this IActionResult result)
        {
            return result.VerifyResponseObjectType<ViewResult>();
        }

        public static RedirectResult VerifyReturnsRedirect(this IActionResult result)
        {
            return result.VerifyResponseObjectType<RedirectResult>();
        }

        public static RedirectToActionResult VerifyReturnsRedirectToActionResult(this IActionResult result)
        {
            return result.VerifyResponseObjectType<RedirectToActionResult>();
        }

        public static RedirectToRouteResult VerifyReturnsRedirectToRouteResult(this IActionResult result)
        {
            return result.VerifyResponseObjectType<RedirectToRouteResult>();
        }

        public static RedirectToActionResult WithActionName(this RedirectToActionResult result, string expectedName)
        {
            Assert.That(result.ActionName, Is.EqualTo(expectedName));
            return result;
        }

        public static RedirectToActionResult WithControllerName(this RedirectToActionResult result, string expectedName)
        {
            Assert.That(result.ControllerName, Is.EqualTo(expectedName));
            return result;
        }

        public static RedirectToRouteResult WithRouteName(this RedirectToRouteResult result, string expectedName)
        {
            Assert.That(result.RouteName, Is.EqualTo(expectedName));
            return result;
        }

        private static TExpectedResponseType VerifyResponseObjectType<TExpectedResponseType>(this IActionResult result) where TExpectedResponseType : IActionResult
        {
            Assert.IsTrue(result is TExpectedResponseType, $"Expected response type {typeof(TExpectedResponseType)} but got {result.GetType()}");
            return (TExpectedResponseType)result;
        }

        public static RedirectResult WithUrl(this RedirectResult result, string expectedUrl)
        {
            Assert.That(result.Url, Is.EqualTo(expectedUrl));
            return result;
        }

        public static TExpectedModel WithModel<TExpectedModel>(this ViewResult result) where TExpectedModel : class
        {
            Assert.IsInstanceOf<TExpectedModel>(result.Model);
            return result.Model as TExpectedModel;
        }
    }
}