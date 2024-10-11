using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Services;

[TestFixture]
public class TempDataStorageServiceTests
{
    private TempDataStorageService _storageService;
    private Mock<IHttpContextAccessor> _httpContextAccessor;
    private Mock<ITempDataDictionaryFactory> _tempDataDictionaryFactory;
    private HttpContext _httpContext;
    private Mock<ITempDataDictionary> _tempDataDictionary;
    private TestCacheObject _cacheObject;

    [SetUp]
    public void Setup()
    {
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _tempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
        _tempDataDictionary = new Mock<ITempDataDictionary>();
        _cacheObject = new TestCacheObject{TestProperty = "This_is_a_test"};

        _httpContext = new DefaultHttpContext();

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(_httpContext);

        _tempDataDictionaryFactory.Setup(x => x.GetTempData(It.Is<HttpContext>(c => c == _httpContext)))
            .Returns(_tempDataDictionary.Object);

        _tempDataDictionary.Setup(x => x.Peek(nameof(TestCacheObject)))
            .Returns(JsonConvert.SerializeObject(_cacheObject));

        _storageService = new TempDataStorageService(_httpContextAccessor.Object, _tempDataDictionaryFactory.Object);
    }

    [Test]
    public void Retrieve_Returns_Expected_Object()
    {
        var result = _storageService.RetrieveFromCache<TestCacheObject>();
        result.TestProperty.Should().Be(_cacheObject.TestProperty);
    }

    private class TestCacheObject
    {
        public string TestProperty { get; init; }
    }
}