using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using System;
using ApprenticeshipStatus = SFA.DAS.CommitmentsV2.Types.ApprenticeshipStatus;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class TrainingDatesViewModel : IAuthorizationContextModel
    {
        public TrainingDatesViewModel()
        {
            StartDate = new MonthYearModel("");
            EndDate = new MonthYearModel("");
            EmploymentEndDate = new MonthYearModel("");
        }

        public long AccountLegalEntityId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public long ProviderId { get; set; }
        public ApprenticeshipStatus ApprenticeshipStatus { get; set; }
        public string Uln { get; set; }
        public int? EmploymentPrice { get; set; }
        public int? Price { get; set; }
        public MonthYearModel StartDate { get; set; }
        public DateTime? StopDate { get; set; }

        [SuppressArgumentException(nameof(StartDate), "You must enter a valid date, for example 09 2022")]
        public int? StartMonth { get => StartDate.Month; set => StartDate.Month = value; }
        
        [SuppressArgumentException(nameof(StartDate), "You must enter a valid date, for example 09 2022")]
        public int? StartYear { get => StartDate.Year; set => StartDate.Year = value; }

        public bool InEditMode { get; set; }
        public string LegalEntityName { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
        public Guid CacheKey { get; set; }
        public bool SkippedDeliveryModelSelection { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
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
    }
}
