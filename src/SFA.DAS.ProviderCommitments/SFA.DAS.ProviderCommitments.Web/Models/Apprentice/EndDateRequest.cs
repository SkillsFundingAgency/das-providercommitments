using System;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class EndDateRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string StartDate { get; set; }
        public string EmploymentEndDate { get; set; }
        public string EndDate { get; set; }
        public int? EmploymentPrice { get; set; }
        public int? Price { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public Guid CacheKey { get; set; }
    }
}
