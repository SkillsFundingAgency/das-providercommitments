namespace SFA.DAS.ProviderCommitments.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IntegerLengthCheckAttribute : Attribute
    {
        public string PropertyName { get; private set; }
        public string DisplayName { get; private set; }
        public int MaxNumberOfDigits { get; private set; }
        public string CustomLengthErrorMessage { get; private set; }

        public IntegerLengthCheckAttribute(string propertyName, string displayName, int maxNumberOfDigits)
        {
            PropertyName = propertyName;
            DisplayName = displayName;
            CustomLengthErrorMessage = $"{displayName} must be {maxNumberOfDigits} numbers or fewer";
            MaxNumberOfDigits = maxNumberOfDigits;
        }
    }
}
