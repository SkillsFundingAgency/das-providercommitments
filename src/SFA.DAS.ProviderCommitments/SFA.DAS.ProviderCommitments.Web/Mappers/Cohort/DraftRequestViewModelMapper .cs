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
    public class DraftRequestViewModelMapper : IMapper<CohortsByProviderRequest, DraftViewModel>
    {
        private readonly IEncodingService _encodingService;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public DraftRequestViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingSummary)
        {
            _encodingService = encodingSummary;
            _commitmentsApiClient = commitmentApiClient;
        }

        public async Task<DraftViewModel> Map(CohortsByProviderRequest source)
        {
            var cohortsResponse = await _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });

            var reviewViewModel = new DraftViewModel
            {
                ProviderId = source.ProviderId,
                Cohorts = cohortsResponse.Cohorts
                    .Where(x => x.GetStatus() == CohortStatus.Draft)
                    .OrderBy(z => z.CreatedOn)
                    .Select(y => new DraftCohortSummaryViewModel
                    {
                        CohortReference = _encodingService.Encode(y.CohortId, EncodingType.CohortReference),
                        EmployerName = y.LegalEntityName,
                        NumberOfApprentices = y.NumberOfDraftApprentices
                    }).ToList()
            };

            return reviewViewModel;
        }
    }
}
