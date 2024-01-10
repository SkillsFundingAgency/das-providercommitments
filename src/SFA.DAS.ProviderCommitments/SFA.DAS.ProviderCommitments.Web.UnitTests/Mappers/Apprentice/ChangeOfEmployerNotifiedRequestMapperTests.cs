using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class ChangeOfEmployerNotifiedRequestMapperTests
    {
        private ChangeOfEmployerNotifiedRequestMapper _mapper;
        private OverlapOptionsForChangeEmployerViewModel _source;
        private Mock<ICacheStorageService> _cacheStorage;
        private Mock<IOuterApiClient> _outerApi;
        private Mock<IAuthenticationService> _authenticationService;
        private ChangeEmployerCacheItem _cacheItem;
        private PostCreateChangeOfEmployerRequest _apiRequest;
        private string _userId;
        private string _userEmail;
        private string _userName;

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();

            _source = fixture.Create<OverlapOptionsForChangeEmployerViewModel>();
            _userId = fixture.Create<string>();
            _userEmail = fixture.Create<string>();
            _userName = fixture.Create<string>();

            _outerApi = new Mock<IOuterApiClient>();
            _outerApi.Setup(x => x.Post<PostCreateChangeOfEmployerResponse>(It.IsAny<PostCreateChangeOfEmployerRequest>()))
                .Callback<IPostApiRequest>(r => _apiRequest = (PostCreateChangeOfEmployerRequest)r)
                .ReturnsAsync(() => new PostCreateChangeOfEmployerResponse());

            _authenticationService = new Mock<IAuthenticationService>();
            _authenticationService.Setup(x => x.UserEmail).Returns(_userEmail);
            _authenticationService.Setup(x => x.UserId).Returns(_userId);
            _authenticationService.Setup(x => x.UserEmail).Returns(_userName);

            _cacheItem = fixture
                .Build<ChangeEmployerCacheItem>()
                .With(x => x.StartDate, "092022")
                .With(x => x.EndDate, "092023")
                .With(x => x.EmploymentEndDate, string.Empty)
                .Create();
            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x => x.RetrieveFromCache<ChangeEmployerCacheItem>
                        (It.Is<Guid>(key => key == _source.CacheKey)))
                .ReturnsAsync(_cacheItem);

            _mapper = new ChangeOfEmployerNotifiedRequestMapper(_outerApi.Object, _cacheStorage.Object, _authenticationService.Object);
        }

        [Test]
        public async Task Map_ApprenticeshipId_Is_Mapped_Correctly()
        {
            await _mapper.Map(_source);
            Assert.That(_apiRequest.ApprenticeshipId, Is.EqualTo(_source.ApprenticeshipId));
        }

        [Test]
        public async Task Map_ProviderId_Is_Mapped_Correctly()
        {
            await _mapper.Map(_source);
            Assert.That(_apiRequest.ProviderId, Is.EqualTo(_source.ProviderId));
        }

        [Test]
        public async Task Map_Price_Is_Mapped_Correctly()
        {
            await _mapper.Map(_source);
            var data = _apiRequest.Data as PostCreateChangeOfEmployerRequest.Body;
            Assert.That(data.Price, Is.EqualTo(_cacheItem.Price));
        }

        [Test]
        public async Task Map_StartDate_Is_Mapped_Correctly()
        {
            await _mapper.Map(_source);
            var data = _apiRequest.Data as PostCreateChangeOfEmployerRequest.Body;
            Assert.That(data.StartDate.Value.ToString("MMyyyy"), Is.EqualTo(_cacheItem.StartDate));
        }

        [Test]
        public async Task Map_EndDate_Is_Mapped_Correctly()
        {
            await _mapper.Map(_source);
            var data = _apiRequest.Data as PostCreateChangeOfEmployerRequest.Body;
            Assert.That(data.EndDate.Value.ToString("MMyyyy"), Is.EqualTo(_cacheItem.EndDate));
        }

        [Test]
        public async Task Map_HasOverlappingTrainingDates_Is_Set_True()
        {
            await _mapper.Map(_source);
            var data = _apiRequest.Data as PostCreateChangeOfEmployerRequest.Body;
            Assert.That(data.HasOverlappingTrainingDates, Is.EqualTo(true));
        }
    }
}
