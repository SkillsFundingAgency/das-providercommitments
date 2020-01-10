using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using SFA.DAS.Commitments.Shared.Extensions;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ApprenticeshipDetailsToViewModelMapper : IMapper<ApprenticeshipDetails, ApprenticeshipDetailsViewModel>
    {
        private readonly IEncodingService _encodingService;

        public ApprenticeshipDetailsToViewModelMapper(IEncodingService encodingService)
        {
            _encodingService = encodingService;
        }

        public Task<ApprenticeshipDetailsViewModel> Map(ApprenticeshipDetails source)
        {
            var result = new ApprenticeshipDetailsViewModel
            {
                EncodedApprenticeshipId = _encodingService.Encode(source.Id, EncodingType.ApprenticeshipId),
                ApprenticeName = $"{source.ApprenticeFirstName} {source.ApprenticeLastName}",
                Uln = source.Uln,
                EmployerName = source.EmployerName,
                CourseName = source.CourseName,
                PlannedStartDate = source.PlannedStartDate.ToGdsFormatWithoutDay(),
                PlannedEndDate = source.PlannedEndDateTime.ToGdsFormatWithoutDay(),
                Status = source.PaymentStatus.ToString(),
                Alerts = new HtmlString(source.Alerts.Any() ? source.Alerts.Aggregate((a,b)=> $"{a}<br/>{b}") : "") 
            };

            return Task.FromResult(result);
        }
    }
}