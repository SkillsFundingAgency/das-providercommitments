using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class DetailsViewModelToAcknowledgementRequestMapper : IMapper<DetailsViewModel, AcknowledgementRequest>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly IAuthenticationService _authenticationService;

        public DetailsViewModelToAcknowledgementRequestMapper(IOuterApiClient outerApiClient, IAuthenticationService authenticationService)
        {
            _outerApiClient = outerApiClient;
            _authenticationService = authenticationService;
        }

        public async Task<AcknowledgementRequest> Map(DetailsViewModel source)
        {
            var apiRequestBody = new PostCohortDetailsRequest.Body
            {
                SubmissionType = source.Selection == CohortDetailsOptions.Approve
                    ? PostCohortDetailsRequest.CohortSubmissionType.Approve
                    : PostCohortDetailsRequest.CohortSubmissionType.Send,
                Message = source.SendMessage,
                UserInfo = new ApimUserInfo
                {
                    UserDisplayName = _authenticationService.UserName,
                    UserEmail = _authenticationService.UserEmail,
                    UserId = _authenticationService.UserId
                }
            };

            var apiRequest = new PostCohortDetailsRequest(source.ProviderId, source.CohortId, apiRequestBody);

            await _outerApiClient.Post<PostCohortDetailsResponse>(apiRequest);

            return new AcknowledgementRequest
            {
                CohortReference = source.CohortReference,
                ProviderId = source.ProviderId,
                SaveStatus = source.Selection == CohortDetailsOptions.Send
                    ? SaveStatus.AmendAndSend
                    :  source.IsApprovedByEmployer && string.IsNullOrEmpty(source.TransferSenderHashedId)
                        ? SaveStatus.Approve
                        : SaveStatus.ApproveAndSend
            };
        }
    }
}
