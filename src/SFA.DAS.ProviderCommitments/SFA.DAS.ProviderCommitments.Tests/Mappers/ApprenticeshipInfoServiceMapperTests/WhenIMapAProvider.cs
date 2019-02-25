using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Mappers;

namespace SFA.DAS.ProviderCommitments.Tests.Mappers.ApprenticeshipInfoServiceMapperTests
{
    [TestFixture]
    public class WhenIMapAProvider
    {
        private ApprenticeshipInfoServiceMapper _mapper;
        private Apprenticeships.Api.Types.Providers.Provider _provider;
        private Func<ProvidersView> _act;

        [SetUp]
        public void Arrange()
        {
            _mapper = new ApprenticeshipInfoServiceMapper(Mock.Of<ICurrentDateTime>());

            _provider = new Apprenticeships.Api.Types.Providers.Provider
            {
                Ukprn = 12345678,
                ProviderName = "TestProvider"
            };

            _act = () => _mapper.MapFrom(TestHelper.Clone(_provider));
        }

        [Test]
        public void ThenUkprnIsMappedCorrectly()
        {
            var result = _act();

            Assert.AreEqual(_provider.Ukprn, result.Provider.Ukprn);
        }

        [Test]
        public void ThenProviderNameIsMappedCorrectly()
        {
            var result = _act();

            Assert.AreEqual(_provider.ProviderName, result.Provider.ProviderName);
        }
    }
}