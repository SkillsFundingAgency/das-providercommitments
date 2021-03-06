﻿using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.ProviderCommitments.Web.Attributes;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class PriceViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string LegalEntityName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        [SuppressArgumentException(nameof(Price), "Total agreed apprenticeship price must be 7 numbers or fewer")]
        public int? Price { get; set; }
        public bool InEditMode { get; set; }
    }
}
