using System;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using ApprenticeshipStatus = SFA.DAS.CommitmentsV2.Types.ApprenticeshipStatus;
using DeliveryModel = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types.DeliveryModel;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class PriceViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public ApprenticeshipStatus ApprenticeshipStatus { get; set; }
        public long ApprenticeshipId { get; set; }
        public string LegalEntityName { get; set; }

        [SuppressArgumentException(nameof(EmploymentPrice),
            "You must enter a valid price. For example, for £1,000 enter 1000")]
        public int? EmploymentPrice { get; set; }

        [SuppressArgumentException(nameof(Price), "You must enter a valid price. For example, for £1,000 enter 1000")]
        public int? Price { get; set; }

        public bool InEditMode { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
        public Guid CacheKey { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? StopDate { get; set; }
    }
}