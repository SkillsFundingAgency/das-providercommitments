using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class WithTransferSenderRequestViewModelMapper : IMapper<CohortsByProviderRequest, WithTransferSenderViewModel>
    {
        private readonly IEncodingService _encodingService;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public WithTransferSenderRequestViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingSummary)
        {
            _encodingService = encodingSummary;
            _commitmentsApiClient = commitmentApiClient;
        }

        public async Task<WithTransferSenderViewModel> Map(CohortsByProviderRequest source)
        {
            var cohortsResponse = await _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });

            var reviewViewModel = new WithTransferSenderViewModel
            {
                ProviderId = source.ProviderId,
                Cohorts = cohortsResponse.Cohorts
                    .Where(x => x.GetStatus() == CohortStatus.WithTransferSender)
                    .OrderBy(GetOrderByDate)
                    .Select(y => new WithTransferSenderCohortSummaryViewModel
                    {
                        CohortReference = _encodingService.Encode(y.CohortId, EncodingType.CohortReference),
                        EmployerName = y.LegalEntityName,
                        NumberOfApprentices = y.NumberOfDraftApprentices
                    }).ToList()
            };

            return reviewViewModel;
        }

        private DateTime GetOrderByDate(CohortSummary s)
        {
            return new[] { s.LatestMessageFromEmployer?.SentOn, s.LatestMessageFromProvider?.SentOn, s.CreatedOn }.Max().Value;
        }
    }
}
