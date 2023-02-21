using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class RouteValueDictionaryExtensions
    {
        public static void AddQueryValuesToRoute(this RouteValueDictionary routeValues, IQueryCollection queryString)
        {
            foreach (string key in queryString.Keys)
            {
                if (!routeValues.ContainsKey(key))
                {
                    routeValues[key] = queryString[key];
                }
            }
        }
    }
}
