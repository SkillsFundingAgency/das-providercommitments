using System;

namespace SFA.DAS.ProviderCommitments.ModelBinding
{
    /// <summary>
    ///     Add this to a class to have its values set from the hashed items provided in the
    ///     request.
    /// </summary>
    [AttributeUsage(validOn:AttributeTargets.Class)]
    public class UnhashAttribute : Attribute
    {

    }
}