namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers;

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
        result.ActionName.Should().Be(expectedName);
        return result;
    }
    
    public static RedirectToActionResult WithRouteValue<T>(this RedirectToActionResult result, string name, T expectedValue)
    {
        result.RouteValues[name].Should().Be(expectedValue);
        return result;
    }

    public static RedirectToActionResult WithControllerName(this RedirectToActionResult result, string expectedName)
    {
        result.ControllerName.Should().Be(expectedName);
        return result;
    }

    public static RedirectToRouteResult WithRouteName(this RedirectToRouteResult result, string expectedName)
    {
        result.RouteName.Should().Be(expectedName);
        return result;
    }

    private static TExpectedResponseType VerifyResponseObjectType<TExpectedResponseType>(this IActionResult result) where TExpectedResponseType : IActionResult
    {
        (result is TExpectedResponseType).Should().BeTrue( $"Expected response type {typeof(TExpectedResponseType)} but got {result.GetType()}");
        return (TExpectedResponseType)result;
    }

    public static RedirectResult WithUrl(this RedirectResult result, string expectedUrl)
    {
        result.Url.Should().Be(expectedUrl);
        return result;
    }

    public static TExpectedModel WithModel<TExpectedModel>(this ViewResult result) where TExpectedModel : class
    {
        result.Model.Should().BeAssignableTo<TExpectedModel>();
        return result.Model as TExpectedModel;
    }
}