using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization.Provider;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization
{
    public class WhenHandlingTrainingProviderAuthorization
    {
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private AuthorizationHandlerContext _context;
        private ClaimsPrincipal _claimsPrincipal;
        private Mock<ICacheStorageService> _cacheStorageService;
        private Mock<IOuterApiService> _outerApiService;
        private TrainingProviderAuthorizationHandler _authorizationHandler;
        private TrainingProviderAllRolesRequirement _requirement;
        private readonly int _ukprn = 123;

        [SetUp]
        public void SetUp()
        {
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _cacheStorageService = new Mock<ICacheStorageService>();
            _outerApiService = new Mock<IOuterApiService>();
            _requirement = new TrainingProviderAllRolesRequirement();
            _authorizationHandler = new TrainingProviderAuthorizationHandler(_outerApiService.Object, _cacheStorageService.Object);

            var claim = new Claim(ProviderClaims.Ukprn, _ukprn.ToString());
            _claimsPrincipal = new ClaimsPrincipal([new ClaimsIdentity([claim])]);
            _context = new AuthorizationHandlerContext([_requirement], _claimsPrincipal, null);

            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        }


        [Test, MoqAutoData]
        public async Task Then_The_ProviderStatus_Is_Valid_And_True_Returned_When_Cache_Returns_Null(ProviderAccountResponse apiResponse)
        {
            //Arrange
            _cacheStorageService.Setup(x => x.SafeRetrieveFromCache<ProviderAccountResponse>(nameof(ProviderAccountResponse)))
                               .ReturnsAsync((ProviderAccountResponse)null);

            apiResponse.CanAccessService = true;

            _outerApiService.Setup(x => x.GetProviderStatus(_ukprn)).ReturnsAsync(apiResponse);

            //Act
            var actual = await _authorizationHandler.IsProviderAuthorized(_context);

            //Assert
            actual.Should().BeTrue();
            _cacheStorageService.Verify(x => x.SaveToCache(nameof(ProviderAccountResponse), apiResponse, 1), Times.Once);

        }

        [Test, MoqAutoData]
        public async Task Then_The_ProviderDetails_Is_InValid_And_False_Returned_When_Cache_Returns_Null(ProviderAccountResponse apiResponse)
        {
            //Arrange
            _cacheStorageService.Setup(x => x.SafeRetrieveFromCache<ProviderAccountResponse>(nameof(ProviderAccountResponse)))
                               .ReturnsAsync((ProviderAccountResponse)null);

            apiResponse.CanAccessService = false;

            _outerApiService.Setup(x => x.GetProviderStatus(_ukprn)).ReturnsAsync(apiResponse);

            //Act
            var actual = await _authorizationHandler.IsProviderAuthorized(_context);

            //Assert
            actual.Should().BeFalse();
            _cacheStorageService.Verify(x => x.SaveToCache(nameof(ProviderAccountResponse), apiResponse, 1), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_ProviderDetails_Is_Null_And_False_Returned_When_Cache_Returns_Null()
        {
            //Arrange
            _cacheStorageService.Setup(x => x.SafeRetrieveFromCache<ProviderAccountResponse>(nameof(ProviderAccountResponse)))
                             .ReturnsAsync((ProviderAccountResponse)null);

            _outerApiService.Setup(x => x.GetProviderStatus(_ukprn)).ReturnsAsync((ProviderAccountResponse)null!);

            //Act
            var actual = await _authorizationHandler.IsProviderAuthorized(_context);

            //Assert
            actual.Should().BeFalse();
            _cacheStorageService.Verify(x => x.SaveToCache(nameof(ProviderAccountResponse), It.IsAny<ProviderAccountResponse>(), 1), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_True_If_Cache_Contains_Data_Is_Valid(ProviderAccountResponse cachedResponse)
        {
            // Arrange
            cachedResponse.CanAccessService = true;

            _cacheStorageService.Setup(x => x.SafeRetrieveFromCache<ProviderAccountResponse>(nameof(ProviderAccountResponse)))
                               .ReturnsAsync(cachedResponse);

            // Act
            var result = await _authorizationHandler.IsProviderAuthorized(_context);

            // Assert
            result.Should().BeTrue();
            _outerApiService.Verify(x => x.GetProviderStatus(It.IsAny<int>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_False_If_Cache_Contains_Data_Is_InValid(ProviderAccountResponse cachedResponse)
        {
            // Arrange
            cachedResponse.CanAccessService = false;

            _cacheStorageService.Setup(x => x.SafeRetrieveFromCache<ProviderAccountResponse>(nameof(ProviderAccountResponse)))
                               .ReturnsAsync(cachedResponse);

            // Act
            var result = await _authorizationHandler.IsProviderAuthorized(_context);

            // Assert
            result.Should().BeFalse();
            _outerApiService.Verify(x => x.GetProviderStatus(It.IsAny<int>()), Times.Never);
        }
    }
}
