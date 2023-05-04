using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers
{
    internal static class VerifyRedirectToPagesExtension
    {
        public static RedirectToActionResult VerifyRedirectsToCohortDetailsPage(this IActionResult result, long providerId, string cohortReference)
        {
            var redirectAction = result
                .VerifyReturnsRedirectToActionResult()
                .WithActionName("Details");

            redirectAction.RouteValues["ProviderId"].Should().Be(providerId);
            redirectAction.RouteValues["CohortReference"].Should().Be(cohortReference);

            return redirectAction;
        }

        public static RedirectToActionResult VerifyRedirectsToRecognisePriorLearningSummaryPage(this IActionResult result, string draftApprenticeshipHashedId)
        {
            var redirectAction = result
                .VerifyReturnsRedirectToActionResult()
                .WithActionName("RecognisePriorLearningSummary");

            redirectAction.RouteValues["DraftApprenticeshipHashedId"].Should().Be(draftApprenticeshipHashedId);

            return redirectAction;
        }

        public static RedirectToActionResult VerifyRedirectsToRecognisePriorLearningPage(this IActionResult result, string draftApprenticeshipHashedId)
        {
            var redirectAction = result
                .VerifyReturnsRedirectToActionResult()
                .WithActionName("RecognisePriorLearning");
            
            redirectAction.RouteValues["DraftApprenticeshipHashedId"].Should().Be(draftApprenticeshipHashedId);
            
            return redirectAction;
        }

        public static RedirectToActionResult VerifyRedirectsToRecognisePriorLearningDetailsPage(this IActionResult result, string draftApprenticeshipHashedId)
        {
            var redirectAction = result
                .VerifyReturnsRedirectToActionResult()
                .WithActionName("RecognisePriorLearningDetails");
            
            redirectAction.RouteValues["DraftApprenticeshipHashedId"].Should().Be(draftApprenticeshipHashedId);
            
            return redirectAction;
        }

        public static RedirectToActionResult VerifyRedirectsToRecognisePriorLearningDataPage(this IActionResult result, string draftApprenticeshipHashedId)
        {
            var redirectAction = result
                .VerifyReturnsRedirectToActionResult()
                .WithActionName("RecognisePriorLearningData");

            redirectAction.RouteValues["DraftApprenticeshipHashedId"].Should().Be(draftApprenticeshipHashedId);

            return redirectAction;
        }

        public static RedirectToActionResult VerifyRedirectsToSelectOptionsPage(this IActionResult result, string draftApprenticeshipHashedId)
        {
            var redirectAction = result
                .VerifyReturnsRedirectToActionResult()
                .WithActionName("SelectOptions");

            redirectAction.RouteValues["DraftApprenticeshipHashedId"].Should().Be(draftApprenticeshipHashedId);

            return redirectAction;
        }
    }
}