using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class ILinkGeneratorExtensions
    {
        public static string CohortDetails(this ILinkGenerator linkGenerator, long providerId, string cohortReference)
        {
            var cohortDetailsUrl = $"{providerId}/apprentices/{cohortReference}/Details";
            return  linkGenerator.ProviderApprenticeshipServiceLink(cohortDetailsUrl);
        }
    }
}
