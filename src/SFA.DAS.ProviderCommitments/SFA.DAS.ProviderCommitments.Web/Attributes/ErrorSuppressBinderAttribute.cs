using System;

namespace SFA.DAS.ProviderCommitments.Web.Attributes
{
    public class ErrorSuppressBinderAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }

        public ErrorSuppressBinderAttribute(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }
    }
}
