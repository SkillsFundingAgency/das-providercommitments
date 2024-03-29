﻿namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest
{
    public class PostValidateDraftApprenticeshipforOverlappingTrainingDateRequest : IPostApiRequest
    {
        public string PostUrl => "OverlappingTrainingDateRequest/validate";

        public object Data { get; set; }
        public PostValidateDraftApprenticeshipforOverlappingTrainingDateRequest(ValidateDraftApprenticeshipApimRequest data)
        {
            Data = data;
        }
    }
}
