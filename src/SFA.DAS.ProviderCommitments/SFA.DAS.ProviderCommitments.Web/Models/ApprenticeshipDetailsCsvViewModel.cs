using System;
using CsvHelper.Configuration.Attributes;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ApprenticeshipDetailsCsvViewModel 
    {
        public static implicit operator ApprenticeshipDetailsCsvViewModel(ApprenticeshipDetails model)
        {
            return new ApprenticeshipDetailsCsvViewModel
            {
                ApprenticeName = model.ApprenticeName,
                Uln = model.Uln,
                Employer = model.EmployerName,
                CourseName = model.CourseName,
                PlannedStartDate = model.PlannedStartDate,
                PlannedEndDate = model.PlannedEndDateTime,
                Status = model.Status,
                Alerts = model.Alerts
            };
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