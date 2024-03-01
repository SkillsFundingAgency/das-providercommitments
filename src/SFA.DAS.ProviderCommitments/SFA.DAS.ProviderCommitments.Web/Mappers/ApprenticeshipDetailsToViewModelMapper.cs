using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ApprenticeshipDetailsToViewModelMapper : IMapper<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDetailsViewModel>
    {
        private readonly IEncodingService _encodingService;

        public ApprenticeshipDetailsToViewModelMapper(IEncodingService encodingService)
        {
            _encodingService = encodingService;
        }

        public Task<ApprenticeshipDetailsViewModel> Map(GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source)
        {
            var result = new ApprenticeshipDetailsViewModel
            {
                EncodedApprenticeshipId = _encodingService.Encode(source.Id, EncodingType.ApprenticeshipId),
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
                IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentPilot
            };

            return Task.FromResult(result);
        }
    }
}