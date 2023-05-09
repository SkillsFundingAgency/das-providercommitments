using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class PostPriorLearningDataRequest : IPostApiRequest
    {
        private readonly long _providerId;
        private readonly long _cohortId;
        private readonly long _draftApprenticeshipId;
        public object Data { get; set; }

        public PostPriorLearningDataRequest(long providerId, long cohortId, long draftApprenticeshipId)
        {
            _providerId = providerId;
            _cohortId = cohortId;
            _draftApprenticeshipId = draftApprenticeshipId;
        }
        public string PostUrl => $"provider/{_providerId}/unapproved/{_cohortId}/apprentices/{_draftApprenticeshipId}/edit/prior-learning-data";
    }
}