using SFA.DAS.Authorization.ModelBinding;
using System;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class AddDraftApprenticeshipViewModel : DraftApprenticeshipViewModel, IAuthorizationContextModel
    {
        public Guid CacheKey { get; set; }
    }
}