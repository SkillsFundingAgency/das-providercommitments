using AutoFixture;
using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers
{
    [TestFixture]
    public class WhenIMapEditDraftApprenticeshipToBaseDraftApprenticeshipRequest
    {
        private EditDraftApprenticeshipToBaseDraftApprenticeshipRequestMapper _mapper;
        private EditDraftApprenticeshipViewModel _source;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Build<EditDraftApprenticeshipViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x => x.StartDate)
                .Without(x => x.EndMonth).Without(x => x.EndYear)
                .Create();

            _mapper = new EditDraftApprenticeshipToBaseDraftApprenticeshipRequestMapper();
        }

        [Test]
        public async Task ProviderIdIsMapped()
        {
            var result = await _mapper.Map(_source);
            result.ProviderId.Should().Be(_source.ProviderId);
        }

        [Test]
        public async Task CohortReferenceIsMapped()
        {
            var result = await _mapper.Map(_source);
            result.CohortReference.Should().Be(_source.CohortReference);
        }

        [Test]
        public async Task DraftApprenticeshipHashedIdIsMapped()
        {
            var result = await _mapper.Map(_source);
            result.DraftApprenticeshipHashedId.Should().Be(_source.DraftApprenticeshipHashedId);
        }
    }
}
