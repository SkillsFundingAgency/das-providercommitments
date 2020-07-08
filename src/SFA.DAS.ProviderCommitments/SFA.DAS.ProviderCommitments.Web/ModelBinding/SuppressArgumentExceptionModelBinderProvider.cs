using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.ModelBinding
{
    public class SuppressArgumentExceptionModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext bindingContext)
        {
            var isArgumentExcpSuppressModelBoundProperty = (bindingContext.Metadata as DefaultModelMetadata)?.Attributes?.PropertyAttributes?.Where(x => x.GetType() == typeof(SuppressArgumentExceptionAttribute))?.Count() > 0;
            if (isArgumentExcpSuppressModelBoundProperty)
            {
                var errorSuppressModelBinder = new SuppressArgumentExceptionModelBinder();
                return errorSuppressModelBinder;
            }

            return null;
        }
    }
}
