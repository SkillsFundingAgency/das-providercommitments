using System;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Models;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ChangeOfEmployerViewModel : IAuthorizationContextModel
    {
        public ChangeOfEmployerViewModel()
        {
            NewStartDate = new MonthYearModel("");
        }
        public long ProviderId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public string EmployerAccountLegalEntityId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }

        public string ApprenticeName { get; set; }
        public string OldEmployerName { get; set; }
        public DateTime OldStartDate { get; set; }
        public DateTime StopDate { get; set; }
        public int OldPrice { get; set; }

        public string NewEmployerName { get; set; }
        public MonthYearModel NewStartDate { get; set; }
        public int NewPrice { get; set; }

    }
}
