using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class AcknowledgementRequestViewModelMapper : IMapper<AcknowledgementRequest, AcknowledgementViewModel>
    {
        private readonly IEncodingService _encodingService;
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IAuthorizationService _authorizationService;

        public AcknowledgementRequestViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingSummary, IAuthorizationService authorizationService)
        {
            _encodingService = encodingSummary;
            _authorizationService = authorizationService;
            _commitmentsApiClient = commitmentApiClient;
        }

        public async Task<AcknowledgementViewModel> Map(AcknowledgementRequest source)
        {
            GetCohortResponse cohort = await _commitmentsApiClient.GetCohort(source.CohortId);

            var result = new AcknowledgementViewModel
            {
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                EmployerName = cohort.LegalEntityName,
                ProviderName = cohort.ProviderName,
                CohortId = source.CohortId,
                Message = cohort.LatestMessageCreatedByProvider,
                WhatHappensNext = new List<string>(),
                WithParty = cohort.WithParty,
                IsTransfer = cohort.TransferSenderId.HasValue,
                CohortApproved = cohort.IsApprovedByProvider && cohort.IsApprovedByProvider,
                ChangeOfPartyRequestId = cohort.ChangeOfPartyRequestId,
                ShowApprenticeEmail  = await _authorizationService.IsAuthorizedAsync(ProviderFeature.ApprenticeEmail),
            };

            switch (source.SaveStatus)
            {
                case SaveStatus.ApproveAndSend when (cohort.TransferSenderId.HasValue && cohort.ChangeOfPartyRequestId == null):
                    result.WhatHappensNext.AddRange(new[]
                    {
                        "The employer will receive your cohort and will either confirm the information is correct or contact you to suggest changes.",
                        "Once the employer approves the cohort, a transfer request will be sent to the funding employer to review.",
                        "You will receive a notification once the funding employer approves or rejects the transfer request. You can view the progress of a request from the 'With transfer sending employers' status screen."
                    });
                    break;
                case SaveStatus.ApproveAndSend:
                    result.WhatHappensNext.Add("The employer will review the cohort and either approve it or contact you with an update.");
                    break;
                default:
                    result.WhatHappensNext.Add("The updated cohort will appear in the employer’s account for them to review.");
                    break;
            }

            if (result.CohortApproved)
            {
                result.PageTitle = "Cohort approved";
            }
            else
            {
                result.PageTitle = source.SaveStatus == SaveStatus.ApproveAndSend
                    ? "Cohort approved and sent to employer"
                    : "Cohort sent to employer for review";
            }

          
            return result;
        }
    }
}
