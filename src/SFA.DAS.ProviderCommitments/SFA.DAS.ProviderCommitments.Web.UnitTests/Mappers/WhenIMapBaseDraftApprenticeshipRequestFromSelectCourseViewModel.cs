using AutoFixture;
using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers
{
    [TestFixture]
    public class WhenIMapBaseDraftApprenticeshipRequestFromSelectCourseViewModel
    {
        private BaseDraftApprenticeshipRequestFromSelectCourseViewModelMapper _mapper;
        private SelectCourseViewModel _source;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<SelectCourseViewModel>();

            _mapper = new BaseDraftApprenticeshipRequestFromSelectCourseViewModelMapper();
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
        public async Task ShowTrainingDetailsIsMapped()
        {
            var result = await _mapper.Map(_source);
            result.ShowTrainingDetails.Should().Be(_source.ShowTrainingDetails);
        }
    }
}
