using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class AcknowledgementRequestViewModelMapper : IMapper<AcknowledgementRequest, AcknowledgementViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public AcknowledgementRequestViewModelMapper(ICommitmentsApiClient commitmentApiClient) => _commitmentsApiClient = commitmentApiClient;

        public async Task<AcknowledgementViewModel> Map(AcknowledgementRequest source)
        {
            var cohort = await _commitmentsApiClient.GetCohort(source.CohortId);

            var result = new AcknowledgementViewModel
            {
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                EmployerName = cohort.LegalEntityName,
                ProviderName = cohort.ProviderName,
                CohortId = source.CohortId,
                Message = string.IsNullOrWhiteSpace(cohort.LatestMessageCreatedByProvider)? "No message added" : cohort.LatestMessageCreatedByProvider,
                WhatHappensNext = new List<string>(),
                WithParty = cohort.WithParty,
                IsTransfer = cohort.TransferSenderId.HasValue,

                CohortApproved = cohort.IsApprovedByProvider && cohort.IsApprovedByEmployer,
                ChangeOfPartyRequestId = cohort.ChangeOfPartyRequestId
            };

            switch (source.SaveStatus)
            {
                case SaveStatus.ApproveAndSend when (cohort.TransferSenderId.HasValue && cohort.ChangeOfPartyRequestId == null):
                    result.WhatHappensNext.AddRange(new[]
                    {
                        "The employer will review the cohort and either approve or contact you with an update.",
                        "Once the employer approves the cohort, a transfer request will be sent to the funding employer to review.",
                        "You’ll receive a notification when the funding employer approves or rejects the transfer request."
                    });
                    break;
                case SaveStatus.ApproveAndSend:
                    result.WhatHappensNext.Add("The employer has 2 weeks to review and approve the cohort. If they don’t do this within 2 weeks, the cohort will automatically go into your ready for review requests.");
                    break;
                default:
                    result.WhatHappensNext.Add("The employer has 2 weeks to review the cohort. If they don’t do this within 2 weeks, the cohort will automatically go into your ready for review requests.");
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
