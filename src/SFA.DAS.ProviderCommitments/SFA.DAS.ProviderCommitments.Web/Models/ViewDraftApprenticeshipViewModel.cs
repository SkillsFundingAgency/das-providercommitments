﻿using System;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ViewDraftApprenticeshipViewModel : IDraftApprenticeshipViewModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Uln { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string TrainingCourse { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Cost { get; set; }
        public string Reference { get; set; }
        public bool ShowEmail { get; set; }
    }
}