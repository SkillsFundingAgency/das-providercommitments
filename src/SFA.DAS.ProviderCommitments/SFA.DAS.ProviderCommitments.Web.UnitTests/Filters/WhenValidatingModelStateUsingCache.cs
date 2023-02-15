﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.Validation.Mvc.Filters;

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
    }
}