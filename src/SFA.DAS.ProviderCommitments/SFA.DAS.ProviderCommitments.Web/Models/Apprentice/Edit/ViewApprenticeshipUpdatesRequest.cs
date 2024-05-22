﻿using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class ViewApprenticeshipUpdatesRequest : IAuthorizationContextModel
    {
        [FromRoute]
        public long ProviderId { get; set; }

        [FromRoute]
        public string ApprenticeshipHashedId { get; set; }

        public long ApprenticeshipId { get; set; }
    }
}
