using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using SFA.DAS.ProviderCommitments.Web.Attributes;

namespace SFA.DAS.ProviderCommitments.Web.ModelBinding
{
    public class IntegerLengthCheckModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
            var converter = TypeDescriptor.GetConverter(bindingContext.ModelType);

            var integerLengthCheckAttribute = (bindingContext.ModelMetadata as DefaultModelMetadata)?
                   .Attributes.PropertyAttributes
                   .OfType<IntegerLengthCheckAttribute>()
                   .FirstOrDefault();

            try
            {
                // throws if a decimal
                var typeCheck = converter.ConvertFrom(valueResult.FirstValue);

                // only check length if it is an integer
                var result = TryParseNullableInt(valueResult.FirstValue);
                if (result.HasValue)
                {
                    if (!HasMoreDigitsThan(result.Value, integerLengthCheckAttribute?.MaxNumberOfDigits ?? int.MaxValue))
                    {
                        bindingContext.Result = ModelBindingResult.Success(result);
                    }
                    else
                    {
                        SetFailureMessageForLength(bindingContext, integerLengthCheckAttribute);
                    }
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                }
            }
            catch (ArgumentException)
            {
                SetFailureMessageIfNotInteger(bindingContext, integerLengthCheckAttribute, valueResult.FirstValue);
            }

            return Task.CompletedTask;
        }

        private static void SetFailureMessageForLength(ModelBindingContext bindingContext,
           IntegerLengthCheckAttribute integerLengthCheckAttribute)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            bindingContext.ModelState.TryAddModelError(
                integerLengthCheckAttribute.PropertyName,
                integerLengthCheckAttribute.CustomLengthErrorMessage);
        }

        private static void SetFailureMessageIfNotInteger(ModelBindingContext bindingContext,
            IntegerLengthCheckAttribute integerLengthCheckAttribute,
            string value)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            bindingContext.ModelState.TryAddModelError(
              integerLengthCheckAttribute.PropertyName,
              $"The value '{value}' is not valid for {integerLengthCheckAttribute.DisplayName}.");
        }

        private static bool HasMoreDigitsThan(int? number, int maxDigits)
        {
            if ( number == null )
            {
                return false;
            }
            
            number = Math.Abs(number.Value);
            int numDigits = number.ToString().Length;
            return numDigits > maxDigits;
        }

        private static int? TryParseNullableInt(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return int.TryParse(input, out int number) ? number : null;
        }
    }
}
