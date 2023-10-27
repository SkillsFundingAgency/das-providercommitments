using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.ProviderCommitments.Web.Attributes
{
    public class RequireQueryParameterAttribute : ActionMethodSelectorAttribute
    {
        public string ValueName { get; private set; }
        
        public RequireQueryParameterAttribute(string valueName)
        {
            ValueName = valueName;
        }
        
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            return routeContext.HttpContext.Request.Query.ContainsKey(ValueName);
        }
    }
}