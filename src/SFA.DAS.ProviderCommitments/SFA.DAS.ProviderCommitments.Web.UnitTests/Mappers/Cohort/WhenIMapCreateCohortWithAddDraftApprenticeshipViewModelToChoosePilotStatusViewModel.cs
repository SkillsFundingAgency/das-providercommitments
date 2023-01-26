using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenIMapCreateCohortWithAddDraftApprenticeshipViewModelToChoosePilotStatusViewModel
    {
        private ChoosePilotStatusViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper _mapper;
        private CreateCohortWithDraftApprenticeshipRequest _request;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Create<CreateCohortWithDraftApprenticeshipRequest>();

            _mapper = new ChoosePilotStatusViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper();
        }

        [TestCase(true, ChoosePilotStatusOptions.Pilot)]
        [TestCase(false, ChoosePilotStatusOptions.NonPilot)]
        [TestCase(null, null)]
        public async Task ThenPilotStatusIsMappedCorrectly(bool? pilotStatus, ChoosePilotStatusOptions? expectedOption)
        {
            _request.IsOnFlexiPaymentPilot = pilotStatus;
            var result = await _mapper.Map(_request);
            Assert.AreEqual(expectedOption, result.Selection);
        }
    }
}