using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class WithEmployerViewModelMapper : IMapper<CohortsByProviderRequest, WithEmployerViewModel>
    {
        private readonly IEncodingService _encodingService;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public WithEmployerViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingSummary)
        {
            _encodingService = encodingSummary;
            _commitmentsApiClient = commitmentApiClient;
        }

        public async Task<WithEmployerViewModel> Map(CohortsByProviderRequest source)
        {
            var cohortsResponse = await _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });

            var withEmployerViewModel = new WithEmployerViewModel
            {
                ProviderId = source.ProviderId,
                Cohorts = cohortsResponse.Cohorts
                    .Where(x => x.GetStatus() == CohortStatus.WithEmployer)
                    .OrderBy(z => z.LatestMessageFromProvider != null ? z.LatestMessageFromProvider.SentOn : z.CreatedOn)
                    .Select(y => new WithEmployerSummaryViewModel
                    {
                        CohortReference = _encodingService.Encode(y.CohortId, EncodingType.CohortReference),
                        EmployerName = y.LegalEntityName,
                        NumberOfApprentices = y.NumberOfDraftApprentices,
                        LastMessage = GetMessage(y.LatestMessageFromProvider)
                    }).ToList()
            };

            return withEmployerViewModel;
        }

        private string GetMessage(Message latestMessageFromProvider)
        {
            if (!string.IsNullOrWhiteSpace(latestMessageFromProvider?.Text))
            {
                return latestMessageFromProvider.Text;
            }

            return "No message added";
        }
    }
}
