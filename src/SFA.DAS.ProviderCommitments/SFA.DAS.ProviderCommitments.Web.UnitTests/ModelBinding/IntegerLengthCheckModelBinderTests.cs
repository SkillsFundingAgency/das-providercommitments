using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;


namespace SFA.DAS.ProviderCommitments.Web.UnitTests.ModelBinding
{
    [TestFixture]
    public class IntegerLengthCheckModelBinderTests
    {

        private const string _displayName = "Total agreed apprenticeship price (excluding VAT)";
        private const string _propertyName = "Cost";
        [Test]
        public async Task BindModelAsync_ShouldReturnSuccess_WhenIntegerWithinLength()
        {
            // Arrange
            var modelBinder = new IntegerLengthCheckModelBinder();
            var modelBindingContext = CreateModelBindingContext("123", typeof(int?), 7);

            // Act
            await modelBinder.BindModelAsync(modelBindingContext);

            // Assert
            modelBindingContext.Result.IsModelSet.Should().BeTrue();
            modelBindingContext.Result.Model.Should().Be(123);
        }

        [Test]
        public async Task BindModelAsync_ShouldReturnFailed_WhenIntegerExceedsLength()
        {
            // Arrange
            var modelBinder = new IntegerLengthCheckModelBinder();
            var modelBindingContext = CreateModelBindingContext("123456", typeof(int?), 7);

            // Act
            await modelBinder.BindModelAsync(modelBindingContext);

            // Assert
            modelBindingContext.Result.IsModelSet.Should().BeFalse();
            modelBindingContext.ModelState.Should().ContainKey(_propertyName);
        }

        [Test]
        public async Task BindModelAsync_ShouldReturnFailed_WhenValueIs_NotInteger()
        {
            // Arrange
            var modelBinder = new IntegerLengthCheckModelBinder();
            var modelBindingContext = CreateModelBindingContext("abc", typeof(int?), 7);

            // Act
            await modelBinder.BindModelAsync(modelBindingContext);

            // Assert
            modelBindingContext.Result.IsModelSet.Should().BeFalse();
            modelBindingContext.ModelState.Should().ContainKey(_propertyName);
        }
         
        [Test]
        public async Task BindModelAsync_ShouldReturnFailed_WhenValue_HasDecimalPlaces()
        {
            // Arrange
            var modelBinder = new IntegerLengthCheckModelBinder();
            var modelBindingContext = CreateModelBindingContext("123.45", typeof(int?), 7);

            // Act
            await modelBinder.BindModelAsync(modelBindingContext);

            // Assert
            modelBindingContext.Result.IsModelSet.Should().BeFalse();
            modelBindingContext.ModelState.Should().ContainKey("PropertyName");
        }
//        private ModelMetadata CreateModelMetadata(Type modelType, int maxNumberOfDigits)
//        {
//            var detailsProvider = new Mock<IMetadataDetailsProvider>();
//            var modelAttributes = new ModelAttributes(new List<object> { new IntegerLengthCheckAttribute("Cost", "Total agreed apprenticeship price (excluding VAT)", maxNumberOfDigits) }, null , null);
//            var identity = ModelMetadataIdentity.ForType(modelType);
//            var details = new DefaultMetadataDetails(identity, modelAttributes);

//            var ttt = DefaultModelMetadataProvider.CreateTypeDetails(identity.Key);


//            var modelMetadata = new DefaultModelMetadata(
//                new Mock<IModelMetadataProvider>().Object,
//                                new Mock<ICompositeMetadataDetailsProvider>().Object,

////                detailsProvider.Object,
//                details);

//            return modelMetadata;
//        }
        //private ModelBindingContext CreateModelBindingContext(string value, Type modelType, int maxNumberOfDigits)
        //{
        //    var valueProvider = new Mock<IValueProvider>();
        //    valueProvider.Setup(v => v.GetValue(It.IsAny<string>())).Returns(new ValueProviderResult(value));

        //    var modelMetadata = new Mock<ModelMetadata>(ModelMetadataIdentity.ForType(modelType));
        //    modelMetadata.Setup(m => m.Attributes).Returns(new ModelAttributes(new List<object>
        //    {
        //        new IntegerLengthCheckAttribute(_propertyName, _displayName,maxNumberOfDigits)                
        //    }));

        //    var modelBindingContext = new DefaultModelBindingContext
        //    {
        //        ModelName = "model",
        //        ModelState = new ModelStateDictionary(),
        //        ModelMetadata = modelMetadata.Object,
        //        ValueProvider = valueProvider.Object
        //    };

        //    return modelBindingContext;
        //}

        private ModelBindingContext CreateModelBindingContext(string value, Type modelType, int maxNumberOfDigits)
        {
            var valueProvider = new Mock<IValueProvider>();
            valueProvider.Setup(vp => vp.GetValue(It.IsAny<string>()))
                         .Returns(new ValueProviderResult(value));

            // Create a mock ModelMetadata and setup its Properties using reflection
            var metadataProvider = new EmptyModelMetadataProvider();
            var modelMetadata = metadataProvider.GetMetadataForType(modelType);

            // Create the attribute and set it in the mock metadata
            var attributes = new Attribute[]
             {
                    new IntegerLengthCheckAttribute(_propertyName, _displayName, maxNumberOfDigits)
             };
         
            var property = typeof(ModelMetadata).GetProperty("Attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (property != null)
            //{
            //    property.SetValue(modelMetadata, new ModelAttributes(attributes));
            //}

            var bindingContext = new DefaultModelBindingContext
            {
                ModelName = _propertyName,
                ValueProvider = valueProvider.Object,
                ModelState = new ModelStateDictionary(),
                ModelMetadata = modelMetadata
            };

         
            return bindingContext;
        }
    }
}
