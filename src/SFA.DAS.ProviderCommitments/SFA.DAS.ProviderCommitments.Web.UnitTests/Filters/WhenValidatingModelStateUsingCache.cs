using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.Validation.Mvc.Filters;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Filters
{
    [TestFixture]
    public class WhenValidatingModelStateUsingCache
    {
        private Mock<ICacheStorageService> _cacheStorageServiceMock;
        private Mock<ValidateModelStateFilter> _validateModelStateFilterMock;
        private CacheFriendlyValidateModelStateFilter _cacheFriendlyValidateModelStateFilterMock;
        private List<IFilterMetadata> _filters;

        [SetUp]
        public void SetUp()
        {
            _cacheStorageServiceMock = new Mock<ICacheStorageService>();
            _validateModelStateFilterMock = new Mock<ValidateModelStateFilter>();
            _cacheFriendlyValidateModelStateFilterMock = new CacheFriendlyValidateModelStateFilter(_cacheStorageServiceMock.Object, _validateModelStateFilterMock.Object);
            _filters = new List<IFilterMetadata>{ new UseCacheForValidationAttribute() };
        }

        [Test]
        public void ThenExistingSharedValidationDoesNotRunOnActionExecuting()
        {
            //Arrange
            var actionExecutingContext = new ActionExecutingContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                _filters,
                new Dictionary<string, object>(),
                null);

            //Act
            _cacheFriendlyValidateModelStateFilterMock.OnActionExecuting(actionExecutingContext);

            //Assert
            _validateModelStateFilterMock.Verify(x => x.OnActionExecuting(actionExecutingContext), Times.Never);
        }

        [Test]
        public void ThenExistingSharedValidationDoesNotRunOnActionExecuted()
        {
            //Arrange
            var actionExecutedContext = new ActionExecutedContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                _filters,
                new Dictionary<string, object>());

            //Act
            _cacheFriendlyValidateModelStateFilterMock.OnActionExecuted(actionExecutedContext);

            //Assert
            _validateModelStateFilterMock.Verify(x => x.OnActionExecuted(actionExecutedContext), Times.Never);
        }

        [Test]
        public void ThenModelStateFromCacheIsMerged()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var modelStateGuid = Guid.NewGuid();
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>(
                new List<KeyValuePair<string, StringValues>>
                {
                    new(CacheKeyConstants.CachedModelStateGuidKey,
                        new StringValues(modelStateGuid.ToString()))
                }));

            var actionExecutingContext = new ActionExecutingContext(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
                _filters,
                new Dictionary<string, object>(),
                null);

            var modelState = new SerializableModelState
            {
                Key = "key1",
                ErrorMessages = new List<string> { "error1" }
            };
            var modelStateDictionary = new SerializableModelStateDictionary();
            modelStateDictionary.Data.Add(modelState);

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<SerializableModelStateDictionary>(modelStateGuid))
                .ReturnsAsync(modelStateDictionary);

            //Act
            _cacheFriendlyValidateModelStateFilterMock.OnActionExecuting(actionExecutingContext);

            //Assert
            actionExecutingContext.ModelState.Keys.Should().Contain("key1");
        }

        [Test]
        public void ThenModelStateIsCleanedFromCache()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var modelStateGuid = Guid.NewGuid();
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>(
                new List<KeyValuePair<string, StringValues>>
                {
                    new(CacheKeyConstants.CachedModelStateGuidKey,
                        new StringValues(modelStateGuid.ToString()))
                }));

            var actionExecutingContext = new ActionExecutingContext(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
                _filters,
                new Dictionary<string, object>(),
                null);

            var modelStateDictionary = new SerializableModelStateDictionary();

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<SerializableModelStateDictionary>(modelStateGuid))
                .ReturnsAsync(modelStateDictionary);

            //Act
            _cacheFriendlyValidateModelStateFilterMock.OnActionExecuting(actionExecutingContext);

            //Assert
            _cacheStorageServiceMock.Verify(x => x.DeleteFromCache(modelStateGuid.ToString()));
        }

        [Test]
        public void ThenCachedErrorsAreAdded()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var modelStateGuid = Guid.NewGuid();
            var cachedErrorGuid = Guid.NewGuid();
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>(
                new List<KeyValuePair<string, StringValues>>
                {
                    new(CacheKeyConstants.CachedErrorGuidKey,
                        new StringValues(cachedErrorGuid.ToString())),
                    new(CacheKeyConstants.CachedModelStateGuidKey,
                        new StringValues(modelStateGuid.ToString()))
                }));

            var controller = new DraftApprenticeshipController(null, null, null, null, null, null,Mock.Of<IAuthenticationService>());

            var actionExecutingContext = new ActionExecutingContext(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
                _filters,
                new Dictionary<string, object>(),
                controller
                );

            var modelStateDictionary = new SerializableModelStateDictionary();

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<SerializableModelStateDictionary>(modelStateGuid))
                .ReturnsAsync(modelStateDictionary);

            var errors = new List<ErrorDetail>
            {
                new("uln", "bogus")
            };

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<List<ErrorDetail>>(cachedErrorGuid))
                .ReturnsAsync(errors);

            //Act
            _cacheFriendlyValidateModelStateFilterMock.OnActionExecuting(actionExecutingContext);

            //Assert
            controller.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public void ThenCachedErrorsAreCleanedFromCache()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var modelStateGuid = Guid.NewGuid();
            var cachedErrorGuid = Guid.NewGuid();
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>(
                new List<KeyValuePair<string, StringValues>>
                {
                    new(CacheKeyConstants.CachedErrorGuidKey,
                        new StringValues(cachedErrorGuid.ToString())),
                    new(CacheKeyConstants.CachedModelStateGuidKey,
                        new StringValues(modelStateGuid.ToString()))
                }));

            var controller = new DraftApprenticeshipController(null, null, null, null, null, null,Mock.Of<IAuthenticationService>());

            var actionExecutingContext = new ActionExecutingContext(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
                _filters,
                new Dictionary<string, object>(),
                controller
            );

            var modelStateDictionary = new SerializableModelStateDictionary();

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<SerializableModelStateDictionary>(modelStateGuid))
                .ReturnsAsync(modelStateDictionary);

            var errors = new List<ErrorDetail>
            {
                new("uln", "bogus")
            };

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<List<ErrorDetail>>(cachedErrorGuid))
                .ReturnsAsync(errors);

            //Act
            _cacheFriendlyValidateModelStateFilterMock.OnActionExecuting(actionExecutingContext);

            //Assert
            _cacheStorageServiceMock.Verify(x => x.DeleteFromCache(cachedErrorGuid.ToString()));
        }
    }
}