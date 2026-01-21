using System;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers
{
    [TestFixture]
    public class WhenIMapEditDraftApprenticeshipToDraftApprenticeshipRequest
    {
        private EditDraftApprenticeshipToDraftApprenticeshipRequestMapper _mapper;
        private EditDraftApprenticeshipViewModel _source;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Build<EditDraftApprenticeshipViewModel>()
                .Without(x => x.BirthDay)
                .Without(x => x.BirthMonth)
                .Without(x => x.BirthYear)
                .Without(x => x.StartMonth)
                .Without(x => x.StartYear)
                .Without(x => x.StartDate)
                .Without(x => x.EndMonth)
                .Without(x => x.EndYear)
                .With(x => x.CohortId, fixture.Create<long>())
                .With(x => x.DraftApprenticeshipId, fixture.Create<long>())
                .Create();

            _mapper = new EditDraftApprenticeshipToDraftApprenticeshipRequestMapper();
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

        [Test]
        public async Task CohortIdIsMapped()
        {
            var result = await _mapper.Map(_source);
            result.CohortId.Should().Be(_source.CohortId.Value);
        }

        [Test]
        public async Task DraftApprenticeshipIdIsMapped()
        {
            var result = await _mapper.Map(_source);
            result.DraftApprenticeshipId.Should().Be(_source.DraftApprenticeshipId.Value);
        }

        [Test]
        public void ThrowsExceptionWhenCohortIdIsNull()
        {
            _source.CohortId = null;
            var action = async () => await _mapper.Map(_source);
            action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("CohortId is required when editing a draft apprenticeship.");
        }

        [Test]
        public void ThrowsExceptionWhenDraftApprenticeshipIdIsNull()
        {
            _source.DraftApprenticeshipId = null;
            var action = async () => await _mapper.Map(_source);
            action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("DraftApprenticeshipId is required when editing a draft apprenticeship.");
        }
    }
}
