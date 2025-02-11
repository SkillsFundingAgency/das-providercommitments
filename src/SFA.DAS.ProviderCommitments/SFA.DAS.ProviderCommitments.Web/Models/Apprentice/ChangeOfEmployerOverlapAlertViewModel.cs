﻿using System.ComponentModel.DataAnnotations;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ChangeOfEmployerOverlapAlertViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
        public string ApprenticeName { get; set; }
        public string Uln { get; set; }
        public string OldEmployerName { get; set; }
        public DateTime OldStartDate { get; set; }
        public DateTime OldEndDate { get; set; }
        public DateTime? StopDate { get; set; }
        public int OldPrice { get; set; }
        public int? OldEmploymentPrice { get; set; }
        public DateTime? OldEmploymentEndDate { get; set; }

        public string NewEmployerName { get; set; }
        public string NewStartDate { get; set; }

        public DateTime NewStartDateTime => string.IsNullOrWhiteSpace(NewStartDate)
            ? DateTime.MinValue
            : new MonthYearModel(NewStartDate).Date.Value;

        public string NewEndDate { get; set; }

        public DateTime NewEndDateTime => string.IsNullOrWhiteSpace(NewEndDate)
            ? DateTime.MinValue
            : new MonthYearModel(NewEndDate).Date.Value;

        public string NewEmploymentEndDate { get; set; }

        public DateTime? NewEmploymentEndDateTime
            => NewEmploymentEndDate == null ? default : new MonthYearModel(NewEmploymentEndDate).Date.Value;


        public int NewPrice { get; set; }
        public int? NewEmploymentPrice { get; set; }
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

        public Guid CacheKey { get; set; }
        public bool ShowDeliveryModel { get; set; }
        public bool ShowDeliveryModelChangeLink { get; set; }
        public CommitmentsV2.Types.DeliveryModel OldDeliveryModel { get; set; }
        public CommitmentsV2.Types.ApprenticeshipStatus Status { get; set; }
    }
}