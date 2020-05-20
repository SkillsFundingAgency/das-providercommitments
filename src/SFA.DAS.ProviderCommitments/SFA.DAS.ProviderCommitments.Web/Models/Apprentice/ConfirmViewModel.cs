using System;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Models;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ConfirmViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }

        public string ApprenticeName { get; set; }
        public string OldEmployerName { get; set; }
        public DateTime OldStartDate { get; set; }
        public DateTime StopDate { get; set; }
        public int OldPrice { get; set; }

        public string NewEmployerName { get; set; }
        public string NewStartDate { get; set; }
        public DateTime NewStartDateTime => new MonthYearModel(NewStartDate).Date.Value;

        public int NewPrice { get; set; }
        public int? FundingBandCap { get; set; }
        public bool ExceedsFundingBandCap
        {
            get
            {
                if (FundingBandCap.HasValue)
                {
                    return NewPrice > FundingBandCap.Value;
                }

                return false;
            }
        }
    }
}
