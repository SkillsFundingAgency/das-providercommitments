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
    public class ApprenticeshipDetailsToViewModelMapper : IMapper<ApprenticeshipDetailsResponse, ApprenticeshipDetailsViewModel>
    {
        private readonly IEncodingService _encodingService;

        public ApprenticeshipDetailsToViewModelMapper(IEncodingService encodingService)
        {
            _encodingService = encodingService;
        }

        public Task<ApprenticeshipDetailsViewModel> Map(ApprenticeshipDetailsResponse source)
        {
            var result = new ApprenticeshipDetailsViewModel
            {
                EncodedApprenticeshipId = _encodingService.Encode(source.Id, EncodingType.ApprenticeshipId),
                ApprenticeName = $"{source.FirstName} {source.LastName}",
                Uln = source.Uln,
                EmployerName = source.EmployerName,
                CourseName = source.CourseName,
                PlannedStartDate = source.StartDate.ToGdsFormatWithoutDay(),
                PlannedEndDate = source.EndDate.ToGdsFormatWithoutDay(),
                Status = source.PaymentStatus.ToString(),
                Alerts = new HtmlString(source.Alerts.Any() ? source.Alerts.Aggregate((a,b)=> $"{a}<br/>{b}") : "") 
            };

            return Task.FromResult(result);
        }
    }
}