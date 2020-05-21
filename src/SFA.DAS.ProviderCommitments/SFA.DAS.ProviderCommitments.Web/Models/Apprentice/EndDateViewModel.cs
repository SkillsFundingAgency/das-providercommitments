using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Models;
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
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long ProviderId { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDateTime => new MonthYearModel(StartDate).Date.Value;
        public int? Price { get; set; }
        public MonthYearModel EndDate { get; set; }
        public int? EndMonth { get => EndDate.Month; set => EndDate.Month = value; }
        public int? EndYear { get => EndDate.Year; set => EndDate.Year = value; }
        public bool InEditMode => Price.HasValue;
    }
}