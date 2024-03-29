﻿using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
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
        public ConfirmationStatus? ConfirmationStatus { get; set; }
        public ApprenticeshipStatus Status { get; set; }
        public IEnumerable<string> Alerts { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public bool? IsOnFlexiPaymentPilot { get; set; }
    }
}