using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using SFA.DAS.ProviderCommitments.ModelBinding.Interfaces;
using SFA.DAS.ProviderCommitments.ModelBinding.Models;

namespace SFA.DAS.ProviderCommitments.ModelBinding.ModelBinder
{
    /// <summary>
    ///     Returns a model provider that can provide un-hashed values to a model.
    /// </summary>
    public class UnhashingModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType.GetCustomAttribute(typeof(UnhashAttribute)) != null)
            {
                return new BinderTypeModelBinder(typeof(UnhashingModelBinder));
            }
            else
            {
                return null;
            }
        }
    }
}