using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using SFA.DAS.ProviderCommitments.Web.Attributes;

namespace SFA.DAS.ProviderCommitments.Web.ModelBinding
{
    public class IntegerLengthCheckModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext bindingContext)
        {
            var isIntegerLengthCheckModelBoundProperty = (bindingContext.Metadata as DefaultModelMetadata)?.Attributes?.PropertyAttributes?.Where(x => x.GetType() == typeof(IntegerLengthCheckAttribute))?.Count() > 0;
            if (isIntegerLengthCheckModelBoundProperty)
            {
                var integerLengthCheckModelBinder = new IntegerLengthCheckModelBinder();
                return integerLengthCheckModelBinder;
            }

            return null;
        }
    }
}
