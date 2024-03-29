using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class OverlapOptionsForChangeEmployerRequestMapperTests
    {
        private OverlapOptionsForChangeEmployerRequestMapper mapper;
        ChangeOfEmployerOverlapAlertViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();

            viewModel = fixture.Build<ChangeOfEmployerOverlapAlertViewModel>()
                .With(x => x.DetailsAcknowledgement, true)
                .Create();

            mapper = new OverlapOptionsForChangeEmployerRequestMapper();
        }

        [Test, MoqAutoData]
        public async Task ApprenticeshipId_IsMapped()
        {
            var result = await mapper.Map(viewModel);
            Assert.That(result.ApprenticeshipId, Is.EqualTo(viewModel.ApprenticeshipId));
        }

        [Test, MoqAutoData]
        public async Task ApprenticeshipHashedId_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.That(result.ApprenticeshipHashedId, Is.EqualTo(viewModel.ApprenticeshipHashedId));
        }

        [Test, MoqAutoData]
        public async Task ProviderId_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.That(result.ProviderId, Is.EqualTo(viewModel.ProviderId));
        }

        [Test, MoqAutoData]
        public async Task CacheKey_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.That(result.CacheKey, Is.EqualTo(viewModel.CacheKey));
        }
    }
}