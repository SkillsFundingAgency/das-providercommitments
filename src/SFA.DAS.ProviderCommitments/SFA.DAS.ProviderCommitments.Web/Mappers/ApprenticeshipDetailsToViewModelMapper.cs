using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using SFA.DAS.Commitments.Shared.Extensions;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;

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
                Status = source.PaymentStatus.ToString(),
                Alerts = new HtmlString(source.Alerts.Any() ? GenerateAlerts(source.Alerts) : "") 
            };

            return Task.FromResult(result);
        }

        private static string GenerateAlerts(IEnumerable<Alerts> alerts)
        {
            var alertString = string.Empty;

            foreach (var alert in alerts)
            {
                if (!string.IsNullOrWhiteSpace(alertString))
                {
                    alertString += "|";
                }
                alertString += alert.FormatAlert();
            }

            return alertString;
        }
    }
}