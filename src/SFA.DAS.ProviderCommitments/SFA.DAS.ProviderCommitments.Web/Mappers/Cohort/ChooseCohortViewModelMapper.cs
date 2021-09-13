using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class ChooseCohortViewModelMapper : IMapper<CohortsByProviderRequest, ChooseCohortViewModel>
    {
        private readonly IEncodingService _encodingService;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ChooseCohortViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingSummary)
        {
            _encodingService = encodingSummary;
            _commitmentsApiClient = commitmentApiClient;
        }

        public async Task<ChooseCohortViewModel> Map(CohortsByProviderRequest source)
        {
            var cohortsResponse = await _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });

            var chooseCohortViewModel = new ChooseCohortViewModel
            {
                ProviderId = source.ProviderId,
                Cohorts = cohortsResponse.Cohorts
                    .Where(x => x.GetStatus() == CohortStatus.Draft || x.GetStatus() == CohortStatus.Review)
                    .OrderBy(z => z.CreatedOn)
                    .Select(y => new ChooseCohortSummaryViewModel
                    {
                        EmployerName = y.LegalEntityName,
                        CohortReference = _encodingService.Encode(y.CohortId, EncodingType.CohortReference),
                        Status = y.GetStatus() == CohortStatus.Review ? "Ready to review" : "Draft",
                        NumberOfApprentices = y.NumberOfDraftApprentices,
                        AccountLegalEntityPublicHashedId = y.AccountLegalEntityPublicHashedId
                    }).ToList(),
                FilterModel = new ChooseCohortFilterModel()

            };

            return chooseCohortViewModel;
        }
    }
}
