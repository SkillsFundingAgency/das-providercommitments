using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration.Attributes;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using ApprenticeshipDetailsResponse = SFA.DAS.CommitmentsV2.Api.Types.Responses.GetApprenticeshipsResponse.ApprenticeshipDetailsResponse;


namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ApprenticeshipDetailsCsvViewModel 
    {
        public static implicit operator ApprenticeshipDetailsCsvViewModel(ApprenticeshipDetailsResponse model)
        {
            return new ApprenticeshipDetailsCsvViewModel
            {
                ApprenticeName = $"{model.FirstName} {model.LastName}",
                Uln = model.Uln,
                Employer = model.EmployerName,
                CourseName = model.CourseName,
                PlannedStartDate = model.StartDate,
                PlannedEndDate = model.EndDate,
                Status = model.PaymentStatus.ToString(),
                Alerts = FormatMe(model.Alerts.ToList())
            };
        }

        private static string FormatMe(IEnumerable<Alerts> alerts)
        {
            var alertString = string.Empty;

            foreach (var alert in alerts)
            {
                if (!string.IsNullOrWhiteSpace(alertString))
                {
                    alertString += "|";
                }
                alertString += alert.ToString();
            }

            return alertString;
        }

        [Name("Apprentice Name")]
        public string ApprenticeName { get ; private set ; }

        [Name("ULN")]
        public string Uln { get ; private set ; }

        [Name("Employer")]
        public string Employer { get ; private set ; }

        [Name("Apprenticeship training course")]
        public string CourseName { get ; private set ; }

        [Name("Planned start date")]
        public DateTime PlannedStartDate { get ; private set ; }

        [Name("Planned end date")]
        public DateTime PlannedEndDate { get ; private set ; }

        [Name("Status")]
        public string Status { get ; private set ; }

        [Name("Alerts")]
        public string Alerts { get ; private set ; }
    }
}