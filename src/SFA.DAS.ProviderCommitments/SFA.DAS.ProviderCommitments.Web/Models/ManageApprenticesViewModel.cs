using System;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ManageApprenticesViewModel
    {
        public long? ProviderId { get; set; }
        public IEnumerable<ApprenticesViewModel> Apprenticeships { get; set; }
        public ManageApprenticesFilterModel FilterModel { get; set; }
        public bool ShowPageLinks  => FilterModel.TotalNumberOfApprenticeshipsFound > ProviderCommitmentsWebConstants.NumberOfApprenticesPerSearchPage;

       
    }
    public class ApprenticesViewModel
    {
        public static implicit operator ApprenticesViewModel(GetApprenticeshipsResponse.ApprenticeshipDetailsResponse model)
        {
            return new ApprenticesViewModel
            {
                ApprenticeName = $"{model.FirstName} {model.LastName}",
                Uln = model.Uln,
                Employer = model.EmployerName,
                CourseName = model.CourseName,
                PlannedStartDate = model.StartDate,
                PlannedEndDate = model.EndDate,
                Status = model.ApprenticeshipStatus.FormatStatus(),
                Alerts = GenerateAlerts(model.Alerts.ToList())
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
                alertString += alert.FormatAlert();
            }

            return alertString;
        }
        public string ApprenticeName { get; private set; }

        public string Uln { get; private set; }

        public string Employer { get; private set; }

        public string CourseName { get; private set; }

        public DateTime PlannedStartDate { get; private set; }

        public DateTime PlannedEndDate { get; private set; }

        public string Status { get; private set; }

        public string Alerts { get; private set; }
    }
}