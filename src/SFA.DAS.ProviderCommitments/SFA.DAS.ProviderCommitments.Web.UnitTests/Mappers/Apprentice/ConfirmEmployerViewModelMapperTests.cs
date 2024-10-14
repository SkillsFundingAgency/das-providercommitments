using System;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

[TestFixture]
public class ConfirmEmployerViewModelMapperTests
{
    private ConfirmEmployerViewModelMapper _mapper;
    private ConfirmEmployerRequest _source;
    private Func<Task<ConfirmEmployerViewModel>> _act;
    private GetConfirmEmployerResponse _accountLegalEntityResponse;
    private Mock<IEncodingService> _encodingService;
    private Mock<ICacheStorageService> _cacheStorage;
    private ChangeEmployerCacheItem _cacheItem;
    private string _encodedAccountLegalEntityId;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _accountLegalEntityResponse = fixture.Create<GetConfirmEmployerResponse>();
        _source = fixture.Create<ConfirmEmployerRequest>();

        var icommitmentApiClient = new Mock<IOuterApiClient>();
        icommitmentApiClient.Setup(x => x.Get<GetConfirmEmployerResponse>(It.IsAny<GetConfirmEmployerRequest>())).ReturnsAsync(_accountLegalEntityResponse);

        _cacheItem = fixture.Create<ChangeEmployerCacheItem>();
        _cacheStorage = new Mock<ICacheStorageService>();
        _cacheStorage.Setup(x =>
                x.RetrieveFromCache<ChangeEmployerCacheItem>(It.Is<Guid>(key => key == _source.CacheKey)))
            .ReturnsAsync(_cacheItem);

        _encodedAccountLegalEntityId = fixture.Create<string>();
        _encodingService = new Mock<IEncodingService>();
        _encodingService.Setup(x => x.Encode(_cacheItem.AccountLegalEntityId,
                It.Is<EncodingType>(e => e == EncodingType.PublicAccountLegalEntityId)))
            .Returns(_encodedAccountLegalEntityId);
            
        _mapper = new ConfirmEmployerViewModelMapper(icommitmentApiClient.Object, _cacheStorage.Object, _encodingService.Object);

        _act = async () => await _mapper.Map(_source);
    }

    [Test]
    public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
    {
        var result = await _act();
        result.EmployerAccountLegalEntityPublicHashedId.Should().Be(_encodedAccountLegalEntityId);
    }

    [Test]
    public async Task ThenEmployerAccountNameIsMappedCorrectly()
    {
        var result = await _act();
        result.EmployerAccountName.Should().Be(_accountLegalEntityResponse.AccountName);
    }

    [Test]
    public async Task ThenEmployerAccountLegalEntityNameIsMappedCorrectly()
    {
        var result = await _act();
       result.LegalEntityName.Should().Be(_accountLegalEntityResponse.LegalEntityName);
    }

    [Test]
    public async Task ThenProviderIdMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }
}