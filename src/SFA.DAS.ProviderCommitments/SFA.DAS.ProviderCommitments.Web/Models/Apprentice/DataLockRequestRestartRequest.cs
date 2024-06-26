﻿using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DataLockRequestRestartRequest : IAuthorizationContextModel
    {
        [FromRoute]
        public string AccountHashedId { get; set; }

        public long AccountId { get; set; }

        [FromRoute]
        public string ApprenticeshipHashedId { get; set; }

        public long ApprenticeshipId { get; set; }
    }
}
