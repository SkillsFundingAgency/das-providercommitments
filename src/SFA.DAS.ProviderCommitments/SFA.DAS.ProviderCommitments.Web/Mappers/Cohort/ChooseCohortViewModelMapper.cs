using System.ComponentModel;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class ChooseCohortViewModelMapper : IMapper<ChooseCohortByProviderRequest, ChooseCohortViewModel>
    {
        private readonly IEncodingService _encodingService;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ChooseCohortViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingSummary)
        {
            _encodingService = encodingSummary;
            _commitmentsApiClient = commitmentApiClient;
        }

        public async Task<ChooseCohortViewModel> Map(ChooseCohortByProviderRequest source)
        {
            var cohortsResponse = await _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId }) ??
                                  new GetCohortsResponse(new List<CohortSummary>());

            var filterModel = new ChooseCohortFilterModel
            {
                ReverseSort = source.ReverseSort,
                SortField = source.SortField,
                UseLearnerData = source.UseLearnerData
            };

            var cohorts = cohortsResponse.Cohorts
                .Where(x => (x.GetStatus() == CohortStatus.Draft || x.GetStatus() == CohortStatus.Review) && !x.IsLinkedToChangeOfPartyRequest)
                .Select(y => new ChooseCohortSummaryViewModel
                {
                    EmployerName = y.LegalEntityName,
                    CohortReference = _encodingService.Encode(y.CohortId, EncodingType.CohortReference),
                    Status = y.GetStatus() == CohortStatus.Review ? "Ready to review" : "Draft",
                    NumberOfApprentices = y.NumberOfDraftApprentices,
                    AccountLegalEntityPublicHashedId = y.AccountLegalEntityPublicHashedId,
                    CreatedOn = y.CreatedOn
                }).ToList();

           var sortedCohorts = GetSortedCohorts(filterModel, cohorts);

           var chooseCohortViewModel = new ChooseCohortViewModel
            {
                ProviderId = source.ProviderId,
                Cohorts = sortedCohorts,
                UseLearnerData = source.UseLearnerData,
                FilterModel = filterModel
            };

            return chooseCohortViewModel;
        }

        private static List<ChooseCohortSummaryViewModel> GetSortedCohorts(ChooseCohortFilterModel filterModel, List<ChooseCohortSummaryViewModel> cohorts)
        {
            List<ChooseCohortSummaryViewModel> sortedCohorts;

            if (string.IsNullOrWhiteSpace(filterModel.SortField))
            {
                sortedCohorts = cohorts.OrderBy(z => z.CreatedOn).ToList();
            }
            else
            {
                var prop = TypeDescriptor.GetProperties(typeof(ChooseCohortSummaryViewModel))
                    .Find(filterModel.SortField, true);

                sortedCohorts = filterModel.ReverseSort
                    ? cohorts.OrderByDescending(z => prop.GetValue(z)).ToList()
                    : cohorts.OrderBy(z => prop.GetValue(z)).ToList();
            }

            return sortedCohorts;
        }
    }
}
