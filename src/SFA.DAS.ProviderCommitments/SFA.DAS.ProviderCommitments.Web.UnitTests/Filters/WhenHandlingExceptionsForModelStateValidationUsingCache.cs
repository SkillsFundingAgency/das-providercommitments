using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Filters;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.Validation.Mvc.Extensions;
using FluentAssertions;
using SFA.DAS.Validation.Mvc.ModelBinding;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Filters
{
    [TestFixture]
    public class WhenHandlingExceptionsForModelStateValidationUsingCache
    {
        private CacheFriendlyCommitmentsValidationFilter _cacheFriendlyCommitmentsValidationFilter;
        private Mock<ICacheStorageService> _cacheStorageServiceMock;
        private Mock<DomainExceptionRedirectGetFilterAttribute> _domainExceptionRedirectGetFilterAttributeMock;
        private List<IFilterMetadata> _filters;

        [SetUp]
        public void SetUp()
        {
            _cacheStorageServiceMock = new Mock<ICacheStorageService>();
            _domainExceptionRedirectGetFilterAttributeMock = new Mock<DomainExceptionRedirectGetFilterAttribute>();
            _cacheFriendlyCommitmentsValidationFilter = new CacheFriendlyCommitmentsValidationFilter(_cacheStorageServiceMock.Object, _domainExceptionRedirectGetFilterAttributeMock.Object);
            _filters = new List<IFilterMetadata> { new UseCacheForValidationAttribute() };
        }

        [Test]
        public void ThenDomainExceptionFilterAttributeDoesNotRunOnException()
        {
            //Arrange
            var exceptionContext = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), _filters);

            //Act
            _cacheFriendlyCommitmentsValidationFilter.OnException(exceptionContext);

            //Assert
            _domainExceptionRedirectGetFilterAttributeMock.Verify(x => x.OnException(exceptionContext), Times.Never);
        }

        [Test]
        public void ThenErrorsAreAddedToCacheOnException()
        {
            //Arrange
            var exceptionContext = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), _filters);
            var errors = new List<ErrorDetail>
            {
                new ErrorDetail("uln", "bogus")
            };
            exceptionContext.Exception = new CommitmentsApiModelException(errors);

            //Act
            _cacheFriendlyCommitmentsValidationFilter.OnException(exceptionContext);

            //Assert
            _cacheStorageServiceMock.Verify(x => x.SaveToCache(It.IsAny<Guid>(), errors, 1));
        }

        [Test]
        public void ThenErrorsAreAddedToRouteDataOnException()
        {
            //Arrange
            var exceptionContext = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), _filters);
            var errors = new List<ErrorDetail>
            {
                new ErrorDetail("uln", "bogus")
            };
            exceptionContext.Exception = new CommitmentsApiModelException(errors);

            //Act
            _cacheFriendlyCommitmentsValidationFilter.OnException(exceptionContext);

            //Assert
            exceptionContext.RouteData.Values.ContainsKey(CacheKeyConstants.CachedErrorGuidKey);
            exceptionContext.RouteData.Values[CacheKeyConstants.CachedErrorGuidKey].Should().BeOfType<Guid>();
        }

        [Test]
        public void ThenModelStateIsAddedToCacheOnException()
        {
            //Arrange
            var exceptionContext = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), _filters)
            {
                Exception = new CommitmentsApiModelException(It.IsAny<List<ErrorDetail>>())
            };
            var key = "key1";
            var errorMessage = "error1";
            exceptionContext.ModelState.AddModelError(key, errorMessage);
            var serializableModelStateData = exceptionContext.ModelState.ToSerializable().Data;

            //Act
            _cacheFriendlyCommitmentsValidationFilter.OnException(exceptionContext);

            //Assert
            _cacheStorageServiceMock.Verify(x => x.SaveToCache(
                It.IsAny<Guid>(),
                It.Is<SerializableModelStateDictionary>(
                    x => x.Data.OfType<SerializableModelState>().FirstOrDefault().ErrorMessages.Contains(errorMessage)
                    && x.Data.OfType<SerializableModelState>().FirstOrDefault().Key == key),
                1));
        }

        [Test]
        public void ThenModelStateIsAddedToRouteDataOnException()
        {
            //Arrange
            var exceptionContext = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), _filters)
            {
                Exception = new CommitmentsApiModelException(It.IsAny<List<ErrorDetail>>())
            };

            //Act
            _cacheFriendlyCommitmentsValidationFilter.OnException(exceptionContext);

            //Assert
            exceptionContext.RouteData.Values.ContainsKey(CacheKeyConstants.CachedModelStateGuidKey);
            exceptionContext.RouteData.Values[CacheKeyConstants.CachedModelStateGuidKey].Should().BeOfType<Guid>();
        }

        [Test]
        public void ThenResultRedirectsOnException()
        {
            //Arrange
            var exceptionContext = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), _filters)
            {
                Exception = new CommitmentsApiModelException(It.IsAny<List<ErrorDetail>>())
            };
            exceptionContext.ModelState.AddModelError("key1", "error1");

            //Act
            _cacheFriendlyCommitmentsValidationFilter.OnException(exceptionContext);

            //Assert
            exceptionContext.Result.Should().NotBeNull();
            exceptionContext.Result.Should().BeOfType<RedirectToRouteResult>();
        }
    }
}