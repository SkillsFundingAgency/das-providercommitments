using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers;

public class ApprenticeshipDetailsToViewModelMapper(IEncodingService encodingService)
    : IMapper<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDetailsViewModel>
{
    public Task<ApprenticeshipDetailsViewModel> Map(GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source)
    {
        var result = new ApprenticeshipDetailsViewModel
        {
            EncodedApprenticeshipId = encodingService.Encode(source.Id, EncodingType.ApprenticeshipId),
            ApprenticeName = $"{source.FirstName} {source.LastName}",
            Uln = source.Uln,
            EmployerName = source.EmployerName,
            CourseName = source.CourseName,
            PlannedStartDate = source.StartDate,
            PlannedEndDate = source.EndDate,
            ConfirmationStatus = source.ConfirmationStatus,
            Status = source.ApprenticeshipStatus,
            Alerts = source.Alerts.Select(x => x.GetDescription()),
            ActualStartDate = source.ActualStartDate,
            EmploymentStatus = MapEmploymentStatus(source.EmployerVerificationStatus, source.EmployerVerificationNotes)
        };

        return Task.FromResult(result);
    }

    private static string MapEmploymentStatus(int? status, string notes)
    {
        return status switch
        {
            null => string.Empty,
            0 => "Check Pending",
            2 => "Employed",
            _ => notes switch
            {
                "NinoAndPAYENotFound" => "Not Verified - No PAYE Scheme and invalid NINO",
                "NinoFailure" => "Not Verified - missing or invalid NINO",
                "NinoInvalid" => "Not Verified - missing or invalid NINO",
                "NinoNotFound" => "Not Verified - missing or invalid NINO",
                "PAYENotFound" => "Not Verified - No PAYE Scheme",
                _ => "Not Verified"
            }
        };
    }
}