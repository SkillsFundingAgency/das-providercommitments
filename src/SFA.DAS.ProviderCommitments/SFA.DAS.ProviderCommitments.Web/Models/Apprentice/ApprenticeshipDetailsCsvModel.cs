using System.Collections.Generic;
using CsvHelper.Configuration.Attributes;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ApprenticeshipDetailsCsvModel
    {
        [Name("Apprentice Name")]
        public string ApprenticeName { get ; private set ; }

        [Name("ULN")]
        public string Uln { get ; private set ; }

        [Name("Employer")]
        public string Employer { get ; private set ; }

        [Name("Apprenticeship training course")]
        public string CourseName { get ; private set ; }

        [Name("Planned start date")]
        public string PlannedStartDate { get ; private set ; }

        [Name("Planned end date")]
        public string PlannedEndDate { get ; private set ; }

        [Name("Status")]
        public string Status { get ; private set ; }

        [Name("Alerts")]
        public string Alerts { get ; private set ; }

        public static implicit operator ApprenticeshipDetailsCsvModel(GetApprenticeshipsResponse.ApprenticeshipDetailsResponse model)
        {
            return new ApprenticeshipDetailsCsvModel
            {
                ApprenticeName = $"{model.FirstName} {model.LastName}",
                Uln = model.Uln,
                Employer = model.EmployerName,
                CourseName = model.CourseName,
                PlannedStartDate = model.StartDate.ToGdsFormatWithoutDay(),
                PlannedEndDate = model.EndDate.ToGdsFormatWithoutDay(),
                Status = model.ApprenticeshipStatus.GetDescription(),
                Alerts = GenerateAlerts(model.Alerts)
            };
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
                alertString += alert.GetDescription();
            }

            return alertString;
        }
    }
}