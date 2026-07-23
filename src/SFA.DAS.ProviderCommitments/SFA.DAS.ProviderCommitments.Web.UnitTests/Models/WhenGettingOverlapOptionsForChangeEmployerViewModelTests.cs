using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models;

public class WhenGettingOverlapOptionsForChangeEmployerViewModelTests
{
    [Test]
    public void WhenHasWithdrawnReasonCode_ShouldSelectChangeEmployerOverlapIlrWithdrawnPartial()
    {
        // Arrange
        var model = new OverlapOptionsForChangeEmployerViewModel { HasWithdrawnStatusCode = true };

        // Act
        string partialName = model.GetTargetPartialViewName();

        // Assert
        Assert.That(partialName, Is.EqualTo("_ChangeEmployerOverlapIlrWithdrawnPartial"));
    }

    [Test]
    public void WhenHasWithdrawnReasonCode_ShouldSelectOverlapOptionsForChangeEmployerPartial()
    {
        // Arrange
        var model = new OverlapOptionsForChangeEmployerViewModel { HasWithdrawnStatusCode = false };

        // Act
        string partialName = model.GetTargetPartialViewName();

        // Assert
        Assert.That(partialName, Is.EqualTo("_OverlapOptionsForChangeEmployerPartial"));
    }
}
