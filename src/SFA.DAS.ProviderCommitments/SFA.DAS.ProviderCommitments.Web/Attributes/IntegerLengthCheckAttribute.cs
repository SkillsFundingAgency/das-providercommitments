namespace SFA.DAS.ProviderCommitments.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IntegerLengthCheckAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
        public int MaxNumberOfDigits { get; set; }
        public string CustomLengthErrorMessage { get; set; }

        public IntegerLengthCheckAttribute(string propertyName, string displayName, int maxNumberOfDigits)
        {
            PropertyName = propertyName;
            DisplayName = displayName;
            CustomLengthErrorMessage = $"{displayName} must be {maxNumberOfDigits} numbers or fewer";
            MaxNumberOfDigits = maxNumberOfDigits;
        }
    }
}
