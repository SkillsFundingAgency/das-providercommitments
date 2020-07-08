using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class StartDateViewModel : IAuthorizationContextModel
    {
        public StartDateViewModel()
        {
            StartDate = new MonthYearModel("");
        }
        public long AccountLegalEntityId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long ProviderId { get; set; }
        public string EndDate { get; set; }
        public int? Price { get; set; }
        public MonthYearModel StartDate { get; set; }
        public DateTime? StopDate { get; set; }

        [SuppressArgumentException(nameof(StartDate), "The start date is not valid")]
        public int? StartMonth { get => StartDate.Month; set => StartDate.Month = value; }
        [SuppressArgumentException(nameof(StartDate), "The start date is not valid")]
        public int? StartYear { get => StartDate.Year; set => StartDate.Year = value; }
        public bool InEditMode => Price.HasValue;
        public string LegalEntityName { get; set; }
    }
}