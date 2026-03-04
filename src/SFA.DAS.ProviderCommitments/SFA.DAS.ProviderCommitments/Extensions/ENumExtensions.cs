using System;
using System.ComponentModel;

namespace SFA.DAS.ProviderCommitments.Extensions;

public static class ENumExtensions
{
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static string GetEnumDescription(this Enum value)
    {
        var fi = value.GetType().GetField(value.ToString());
        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes != null && attributes.Length > 0)
        {
            return attributes[0].Description;
        }

        return value.ToString();
    }
}