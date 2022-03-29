﻿namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetCohortRequest : IGetApiRequest
    {
        public long _cohortId;
        public string GetUrl => $"Cohort/{_cohortId}";

        public GetCohortRequest(long cohortId)
        {
            _cohortId = cohortId;
        }
    }
}
