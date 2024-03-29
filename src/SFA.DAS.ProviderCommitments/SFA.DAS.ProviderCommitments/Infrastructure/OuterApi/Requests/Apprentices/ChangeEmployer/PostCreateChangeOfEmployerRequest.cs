﻿using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer
{
    public class PostCreateChangeOfEmployerRequest : IPostApiRequest
    {
        public long ProviderId { get; }
        public long ApprenticeshipId { get; }

        public PostCreateChangeOfEmployerRequest(long providerId, long apprenticeshipId, Body body)
        {
            ProviderId = providerId;
            ApprenticeshipId = apprenticeshipId;
            Data = body;
        }

        public string PostUrl => $"provider/{ProviderId}/apprentices/{ApprenticeshipId}/change-employer/confirm";
        public object Data { get; set; }

        public class Body : ApimSaveDataRequest
        {
            public long AccountLegalEntityId { get; set; }
            public int? Price { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public DateTime? EmploymentEndDate { get; set; }
            public int? EmploymentPrice { get; set; }
            public DeliveryModel? DeliveryModel { get; set; }
            public bool HasOverlappingTrainingDates { get; set; }
        }
    }

    public class PostCreateChangeOfEmployerResponse
    {
    }
}
