using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
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
        public string EmploymentEndDate { get; set; }
        public string EndDate { get; set; }
        public int? EmploymentPrice { get; set; }
        public int? Price { get; set; }
        public MonthYearModel StartDate { get; set; }
        public DateTime? StopDate { get; set; }

        [SuppressArgumentException(nameof(StartDate), "You must enter a valid date, for example 09 2022")]
        public int? StartMonth { get => StartDate.Month; set => StartDate.Month = value; }
        [SuppressArgumentException(nameof(StartDate), "You must enter a valid date, for example 09 2022")]
        public int? StartYear { get => StartDate.Year; set => StartDate.Year = value; }
        public bool InEditMode => Price.HasValue;
        public string LegalEntityName { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public Guid CacheKey { get; set; }
    }
}