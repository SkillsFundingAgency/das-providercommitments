using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.ProviderCommitments.ModelBinding.Interfaces;

namespace SFA.DAS.ProviderCommitments.ModelBinding.ModelBinder
{
    public class UnhashingModelBinder : IModelBinder
    {
        private readonly IHashingContextProvider _hashingContextProvider;

        public UnhashingModelBinder(IHashingContextProvider hashingContextProvider)
        {
            _hashingContextProvider = hashingContextProvider;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var authorizationContext = _hashingContextProvider.GetHashingContext();

            var model = Activator.CreateInstance(bindingContext.ModelType);

            foreach (var property in bindingContext.ModelType.GetProperties())
            {
                if (authorizationContext.TryGet(property.Name, out object value))
                {
                    property.SetValue(model, value);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}