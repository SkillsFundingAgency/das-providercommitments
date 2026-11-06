using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Filters;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Filters
{
    [TestFixture]
    public class WhenHandlingBulkUploadValidationErrors
    {
        private Mock<ICacheService> _cacheService;
        private HandleBulkUploadValidationErrorsAttribute _sut;
        private Guid _cacheId;

        [SetUp]
        public void SetUp()
        {
            _cacheId = Guid.NewGuid();
            _cacheService = new Mock<ICacheService>();
            _cacheService
                .Setup(x => x.SetCache(It.IsAny<List<BulkUploadValidationError>>(), It.IsAny<string>()))
                .ReturnsAsync(_cacheId);
            _sut = new HandleBulkUploadValidationErrorsAttribute(_cacheService.Object);
        }

        [Test]
        public void ThenCommitmentsApiBulkUploadModelExceptionRedirectsToFileUploadValidationErrors()
        {
            var errors = new List<BulkUploadValidationError>
            {
                new(1, "Employer", "123", "Name", [new Error("TrainingTotalHours", "You must enter the total off-the-job training time")])
            };
            var context = CreateExceptionContext(new CommitmentsApiBulkUploadModelException(errors));

            _sut.OnException(context);

            context.RouteData.Values["action"].Should().Be(nameof(CohortController.FileUploadValidationErrors));
            context.RouteData.Values["CachedErrorGuid"].Should().Be(_cacheId.ToString());
            context.Result.Should().BeOfType<RedirectToRouteResult>();
        }

        [Test]
        public void ThenCommitmentsApiModelExceptionRedirectsToFileUploadValidationErrors()
        {
            var context = CreateExceptionContext(new CommitmentsApiModelException(
            [
                new ErrorDetail("PriceReducedBy", "Enter the total price reduction due to RPL")
            ]));

            _sut.OnException(context);

            context.RouteData.Values["action"].Should().Be(nameof(CohortController.FileUploadValidationErrors));
            _cacheService.Verify(x => x.SetCache(
                It.Is<List<BulkUploadValidationError>>(e =>
                    e.Count == 1 &&
                    e[0].RowNumber == 0 &&
                    e[0].Errors[0].Property == "PriceReducedBy"),
                nameof(HandleBulkUploadValidationErrorsAttribute)), Times.Once);
            context.Result.Should().BeOfType<RedirectToRouteResult>();
        }

        [Test]
        public void ThenOtherExceptionsAreIgnored()
        {
            var context = CreateExceptionContext(new InvalidOperationException("boom"));

            _sut.OnException(context);

            context.Result.Should().BeNull();
            _cacheService.Verify(x => x.SetCache(It.IsAny<List<BulkUploadValidationError>>(), It.IsAny<string>()), Times.Never);
        }

        private static ExceptionContext CreateExceptionContext(Exception exception)
        {
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = exception
            };
        }
    }
}
