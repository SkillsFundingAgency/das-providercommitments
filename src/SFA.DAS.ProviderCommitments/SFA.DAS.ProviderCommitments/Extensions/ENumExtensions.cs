using System;

namespace SFA.DAS.ProviderCommitments.Extensions
{
    public static class ENumExtensions
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}