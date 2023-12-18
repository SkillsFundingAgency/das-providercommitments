using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIPostOverlapOptionsForChangeEmployer
    {
        private OverlappingTrainingDateRequestControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new OverlappingTrainingDateRequestControllerTestFixture();
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToSendOverlapEmailToEmployer()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.SendStopRequest)
                .OverlapOptionsForChangeEmployer();

            _fixture.VerifyUserRedirectedTo(ControllerConstants.OverlappingTrainingDateRequestController.Actions.ChangeOfEmployerNotified);
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToContactTheEmployer()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.ContactTheEmployer)
                .OverlapOptionsForChangeEmployer();

            _fixture.VerifyUserRedirectedTo(ControllerConstants.ApprenticeController.Actions.Index, ControllerConstants.ApprenticeController.Name);
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToAddApprenticeshipLater()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.CompleteActionLater)
                .OverlapOptionsForChangeEmployer();

            _fixture.VerifyUserRedirectedTo(ControllerConstants.ApprenticeController.Actions.Index, ControllerConstants.ApprenticeController.Name);
        }
    }
}