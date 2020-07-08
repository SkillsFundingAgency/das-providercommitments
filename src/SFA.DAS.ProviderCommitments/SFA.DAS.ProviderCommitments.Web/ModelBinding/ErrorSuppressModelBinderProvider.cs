using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.ModelBinding
{
    public class ErrorSuppressModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext bindingContext)
        {
            var bind = (bindingContext.Metadata as DefaultModelMetadata);
            var isErrorSuppressModelBoundProperty = (bindingContext.Metadata as DefaultModelMetadata).Attributes.PropertyAttributes?.Where(x => x.GetType() == typeof(ErrorSuppressBinderAttribute))?.Count() > 0;
            if (isErrorSuppressModelBoundProperty)
            {
                var errorSuppressModelBinder = new ErrorSuppressModelBinder();
                return errorSuppressModelBinder;
            }

            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var holderType = bindingContext.Metadata.ContainerType;
            if (holderType != null)
            {
                var propertyType = holderType.GetProperty(bindingContext.Metadata.PropertyName);
                var attributes = propertyType.GetCustomAttributes(true);
                if (attributes.Cast<Attribute>().Any(a => a.GetType().IsEquivalentTo(typeof(ErrorSuppressBinderAttribute))))
                {
                    var errorSuppressModelBinder = new ErrorSuppressModelBinder();
                    return errorSuppressModelBinder;
                }
            }

            return null;
        }
    }
}
