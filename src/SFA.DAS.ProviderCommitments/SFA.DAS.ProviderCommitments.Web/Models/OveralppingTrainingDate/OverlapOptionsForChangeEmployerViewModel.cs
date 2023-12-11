using SFA.DAS.Authorization.ModelBinding;
using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate
{
    public class OverlapOptionsForChangeEmployerViewModel : DraftApprenticeshipOverlapOptionViewModel
    {
        public Guid CacheKey { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long? ApprenticeshipId { get; set; }
    }
}
