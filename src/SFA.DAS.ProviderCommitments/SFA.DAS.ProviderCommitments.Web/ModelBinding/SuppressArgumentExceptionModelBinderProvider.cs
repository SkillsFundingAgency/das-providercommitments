using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.ModelBinding
{
    public class SuppressArgumentExceptionModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext bindingContext)
        {
            var bind = (bindingContext.Metadata as DefaultModelMetadata);
            var isErrorSuppressModelBoundProperty = (bindingContext.Metadata as DefaultModelMetadata)?.Attributes?.PropertyAttributes?.Where(x => x.GetType() == typeof(SuppressArgumentExceptionAttribute))?.Count() > 0;
            if (isErrorSuppressModelBoundProperty)
            {
                var errorSuppressModelBinder = new SuppressArgumentExceptionModelBinder();
                return errorSuppressModelBinder;
            }

            return null;
        }
    }
}
