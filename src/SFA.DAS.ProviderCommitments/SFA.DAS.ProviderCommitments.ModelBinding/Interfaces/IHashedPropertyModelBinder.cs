using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.ModelBinding.ModelBinderValues;

namespace SFA.DAS.ProviderCommitments.ModelBinding.Interfaces
{
    /// <summary>
    ///     Used by <see cref="ModelBindingHashValuesProvider"/> to set model properties
    ///     from route data and query params on the incoming request.
    /// </summary>
    public interface IHashedPropertyModelBinder
    {
        void BindModel(ActionContext context, IHashingValues modelBindingHashValues);
    }
}