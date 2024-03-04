using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Filters;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Filters;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Filters
{
    [TestFixture]
    public class WhenHandlingExceptionsForModelStateValidationUsingTempData
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
            _filters = new List<IFilterMetadata>();
        }

        [Test]
        public void ThenDomainExceptionFilterAttributeDoesNotRunOnException()
        {
            //Arrange
            var exceptionContext = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), _filters);
            var errors = new List<ErrorDetail>
            {
                new("uln", "bogus")
            };
            exceptionContext.Exception = new CommitmentsApiModelException(errors);

            //Act
            _cacheFriendlyCommitmentsValidationFilter.OnException(exceptionContext);

            //Assert
            _domainExceptionRedirectGetFilterAttributeMock.Verify(x => x.OnException(exceptionContext), Times.Once);
        }
    }
}