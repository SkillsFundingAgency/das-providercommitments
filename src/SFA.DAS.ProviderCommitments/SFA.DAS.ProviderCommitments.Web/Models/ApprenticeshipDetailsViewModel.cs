using System;
using Microsoft.AspNetCore.Html;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ApprenticeshipDetailsViewModel
    {
        public string EncodedApprenticeshipId { get; set; }
        public string ApprenticeName { get ; set ; }
        public string Uln { get; set; }
        public string EmployerName { get; set; }
        public string CourseName { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public string Status { get; set; }
        public HtmlString Alerts { get; set; }
    }
}