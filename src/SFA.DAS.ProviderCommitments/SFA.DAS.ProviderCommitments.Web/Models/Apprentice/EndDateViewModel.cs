using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class EndDateViewModel : IAuthorizationContextModel
    {
        public EndDateViewModel()
        {
            EndDate = new MonthYearModel("");
        }
        public long AccountLegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long ProviderId { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDateTime => new MonthYearModel(StartDate).Date.Value;
        public int? EmploymentPrice { get; set; }
        public int? Price { get; set; }

        public MonthYearModel EmploymentEndDate { get; set; }
        [SuppressArgumentException(nameof(EmploymentEndDate), "The employment end date is not valid")]
        public int? EmploymentEndMonth { get => EmploymentEndDate.Month; set => EmploymentEndDate.Month = value; }
        [SuppressArgumentException(nameof(EmploymentEndDate), "The employment end date is not valid")]
        public int? EmploymentEndYear { get => EmploymentEndDate.Year; set => EmploymentEndDate.Year = value; }

        public MonthYearModel EndDate { get; set; }
        [SuppressArgumentException(nameof(EndDate),"The end date is not valid")]
        public int? EndMonth { get => EndDate.Month; set => EndDate.Month = value; }
        [SuppressArgumentException(nameof(EndDate), "The end date is not valid")]
        public int? EndYear { get => EndDate.Year; set => EndDate.Year = value; }
        public bool InEditMode => Price.HasValue;
        public DeliveryModel? DeliveryModel { get; set; }
    }
}