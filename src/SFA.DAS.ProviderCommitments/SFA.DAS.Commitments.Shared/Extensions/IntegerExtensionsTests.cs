namespace SFA.DAS.Commitments.Shared.Extensions
{
    public static class IntegerExtensions
    {
        public static string ToGdsCostFormat(this int value)
        {
            return $"£{value:n0}";
        }
    }
}
