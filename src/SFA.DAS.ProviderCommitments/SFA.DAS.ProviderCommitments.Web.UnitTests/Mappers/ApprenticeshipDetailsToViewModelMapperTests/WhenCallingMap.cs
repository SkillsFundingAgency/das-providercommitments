using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Mappers;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.ApprenticeshipDetailsToViewModelMapperTests;

public class WhenCallingMap
{
    [Test, MoqAutoData]
    public async Task Then_Maps_ApprenticeshipId(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        string encodedApprenticeshipId,
        [Frozen] Mock<IEncodingService> mockEncodingService,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Arrange
        mockEncodingService
            .Setup(service => service.Encode(source.Id, EncodingType.ApprenticeshipId))
            .Returns(encodedApprenticeshipId);

        // Act
        var result = await mapper.Map(source);

        // Assert
        result.EncodedApprenticeshipId.Should().Be(encodedApprenticeshipId);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_ApprenticeName(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Act
        var result = await mapper.Map(source);

        // Assert
        result.ApprenticeName.Should().Be($"{source.FirstName} {source.LastName}");
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_Uln(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Act
        var result = await mapper.Map(source);

        // Assert
        result.Uln.Should().Be(source.Uln);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_EmployerName(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Act
        var result = await mapper.Map(source);

        // Assert
        result.EmployerName.Should().Be(source.EmployerName);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_CourseName(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Act
        var result = await mapper.Map(source);

        // Assert
        result.CourseName.Should().Be(source.CourseName);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_PlannedStartDate(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Act
        var result = await mapper.Map(source);

        // Assert
        result.PlannedStartDate.Should().Be(source.StartDate);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_PlannedEndDate(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Act
        var result = await mapper.Map(source);

        // Assert
        result.PlannedEndDate.Should().Be(source.EndDate);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_ConfirmationStatus(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Act
        var result = await mapper.Map(source);

        // Assert
        result.ConfirmationStatus.Should().Be(source.ConfirmationStatus);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_ApprenticeshipStatus(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Act
        var result = await mapper.Map(source);

        // Assert
        result.Status.Should().Be(source.ApprenticeshipStatus);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_Alerts(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Arrange
        var alertStrings = source.Alerts.Select(x => x.GetDescription());

        // Act
        var result = await mapper.Map(source);

        // Assert
        result.Alerts.Should().BeEquivalentTo(alertStrings);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_ActualStartDate(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Act
        var result = await mapper.Map(source);

        // Assert
        result.ActualStartDate.Should().Be(source.ActualStartDate);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_EmploymentStatus_NullStatus_ReturnsEmpty(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Arrange
        source.EmployerVerificationStatus = null;

        // Act
        var result = await mapper.Map(source);

        // Assert
        result.EmploymentStatus.Should().Be(string.Empty);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_EmploymentStatus_Pending_ReturnsCheckPending(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Arrange
        source.EmployerVerificationStatus = 0;

        // Act
        var result = await mapper.Map(source);

        // Assert
        result.EmploymentStatus.Should().Be("Check Pending");
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_EmploymentStatus_Passed_ReturnsEmployed(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Arrange
        source.EmployerVerificationStatus = 2;

        // Act
        var result = await mapper.Map(source);

        // Assert
        result.EmploymentStatus.Should().Be("Employed");
    }

    [TestCase("HmrcFailure", "Not Verified")]
    [TestCase("NinoFailure", "Not Verified - missing or invalid NINO")]
    [TestCase("NinoInvalid", "Not Verified - missing or invalid NINO")]
    [TestCase("NinoNotFound", "Not Verified - missing or invalid NINO")]
    [TestCase("NinoAndPAYENotFound", "Not Verified - No PAYE Scheme and invalid NINO")]
    [TestCase("PAYENotFound", "Not Verified - No PAYE Scheme")]
    public async Task Then_Maps_EmploymentStatus_ErrorWithCode_ReturnsCorrectNotVerifiedString(string errorCode, string expected)
    {
        // Arrange
        var source = new GetApprenticeshipsResponse.ApprenticeshipDetailsResponse
        {
            EmployerVerificationStatus = 4,
            EmployerVerificationNotes = errorCode,
            Alerts = []
        };
        var mapper = new ApprenticeshipDetailsToViewModelMapper(Mock.Of<IEncodingService>());

        // Act
        var result = await mapper.Map(source);

        // Assert
        result.EmploymentStatus.Should().Be(expected);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_EmploymentStatus_FailedNoNotes_ReturnsNotVerified(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Arrange
        source.EmployerVerificationStatus = 3;
        source.EmployerVerificationNotes = null;

        // Act
        var result = await mapper.Map(source);

        // Assert
        result.EmploymentStatus.Should().Be("Not Verified");
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_EmploymentStatus_UnknownErrorCode_ReturnsNotVerified(
        GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
        ApprenticeshipDetailsToViewModelMapper mapper)
    {
        // Arrange
        source.EmployerVerificationStatus = 4;
        source.EmployerVerificationNotes = "SomeUnknownCode";

        // Act
        var result = await mapper.Map(source);

        // Assert
        result.EmploymentStatus.Should().Be("Not Verified");
    }
}