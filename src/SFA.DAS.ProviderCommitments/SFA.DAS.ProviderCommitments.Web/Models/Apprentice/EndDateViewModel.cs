using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class EndDateViewModel : IAuthorizationContextModel
    {
        public EndDateViewModel()
        {
            EndDate = new MonthYearModel("");
            EmploymentEndDate = new MonthYearModel("");
        }

        public long AccountLegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long ProviderId { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDateTime => new MonthYearModel(StartDate).Date.Value;
        public MonthYearModel EmploymentEndDate { get; set; }
        [SuppressArgumentException(nameof(EmploymentEndDate), "You must enter a valid date, for example 09 2022")]
        public int? EmploymentEndMonth { get => EmploymentEndDate.Month; set => EmploymentEndDate.Month = value; }
        [SuppressArgumentException(nameof(EmploymentEndDate), "You must enter a valid date, for example 09 2022")]
        public int? EmploymentEndYear { get => EmploymentEndDate.Year; set => EmploymentEndDate.Year = value; }

        public MonthYearModel EndDate { get; set; }
        [SuppressArgumentException(nameof(EndDate), "You must enter a valid date, for example 09 2022")]
        public int? EndMonth { get => EndDate.Month; set => EndDate.Month = value; }
        [SuppressArgumentException(nameof(EndDate), "You must enter a valid date, for example 09 2022")]
        public int? EndYear { get => EndDate.Year; set => EndDate.Year = value; }
        public bool InEditMode { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public Guid CacheKey { get; set; }
    }
}