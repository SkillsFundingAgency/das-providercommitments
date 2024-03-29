﻿namespace SFA.DAS.ProviderCommitments.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SuppressArgumentExceptionAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public string CustomErrorMessage { get; set; }

        public SuppressArgumentExceptionAttribute(string propertyName, string customErrorMessage)
        {
            PropertyName = propertyName;
            CustomErrorMessage = customErrorMessage;
        }
    }
}
