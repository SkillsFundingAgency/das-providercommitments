using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Models;
using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ChangeStartDateViewModel : IAuthorizationContextModel
    {
        public ChangeStartDateViewModel()
        {
            StartDate = new MonthYearModel("");
        }

        public long AccountLegalEntityId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long ProviderId { get; set; }
        public MonthYearModel StartDate { get; set; }
        public DateTime? StopDate { get; set; }
    }
}