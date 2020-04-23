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
    public class ReviewRequestViewModelMapper : IMapper<CohortsByProviderRequest, ReviewViewModel2>
    {
        private readonly IEncodingService _encodingService;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ReviewRequestViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingSummary)
        {
            _encodingService = encodingSummary;
            _commitmentsApiClient = commitmentApiClient;
        }

        public async Task<ReviewViewModel2> Map(CohortsByProviderRequest source)
        {
            var cohortsResponse = await _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });

            var reviewViewModel = new ReviewViewModel2
            {
                ProviderId = source.ProviderId,
                Cohorts = cohortsResponse.Cohorts
                    .Where(x => x.GetStatus() == CohortStatus.Review)
                    .OrderByDescending(z => z.CreatedOn)
                    .Select(y => new ReviewCohortSummaryViewModel2
                    {
                        CohortReference = _encodingService.Encode(y.CohortId, EncodingType.CohortReference),
                        EmployerName = y.LegalEntityName,
                        NumberOfApprentices = y.NumberOfDraftApprentices,
                        LastMessage = GetMessage(y.LatestMessageFromEmployer)
                    }).ToList()
            };

            return reviewViewModel;
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
