﻿namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class SelectAddDraftApprenticeshipJourneyViewModel
    {
        public long ProviderId { get; set; }

        public AddDraftApprenticeshipJourneyOptions? Selection { get; set; }
    }

    public enum AddDraftApprenticeshipJourneyOptions
    {
        NewCohort,
        ExistingCohort
    }
}
