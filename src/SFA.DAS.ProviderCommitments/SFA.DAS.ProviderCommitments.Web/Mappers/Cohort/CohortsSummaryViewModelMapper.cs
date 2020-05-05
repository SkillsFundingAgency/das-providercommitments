using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class CohortsSummaryViewModelMapper : IMapper<CohortsByProviderRequest, CohortsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IProviderRelationshipsApiClient _providerRelationshipsApiClient;
        private readonly IUrlHelper _urlHelper;
        private readonly IPasAccountApiClient _pasAccountApiClient;

        public CohortsSummaryViewModelMapper(ICommitmentsApiClient commitmentApiClient, IProviderRelationshipsApiClient providerRelationshipsApiClient,
            IUrlHelper urlHelper, IPasAccountApiClient pasAccountApiClient)
        {
            _commitmentsApiClient = commitmentApiClient;
            _providerRelationshipsApiClient = providerRelationshipsApiClient;
            _urlHelper = urlHelper;
            _pasAccountApiClient = pasAccountApiClient;
        }

        public async Task<CohortsViewModel> Map(CohortsByProviderRequest source)
        {
            async Task<(CohortSummary[] Cohorts, bool HasRelationship, ProviderAgreementStatus providerAgreementStatus)> GetData() 
            {
                var getCohortsTask = _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });
                var hasRelationshipTask = _providerRelationshipsApiClient.HasRelationshipWithPermission(new HasRelationshipWithPermissionRequest { Ukprn = source.ProviderId, Operation = Operation.CreateCohort });
                var providerAgreement = _pasAccountApiClient.GetAgreement(source.ProviderId);

                await Task.WhenAll(getCohortsTask, hasRelationshipTask, providerAgreement);
                return (getCohortsTask.Result.Cohorts, hasRelationshipTask.Result, providerAgreement.Result.Status);
            }

            var data = await GetData();

            return new CohortsViewModel
            {
                ProviderId = source.ProviderId,
                ShowDrafts = (data.HasRelationship),
                CohortsInDraft = new CohortCardLinkViewModel(
                    data.Cohorts.Count(x => x.GetStatus() == CohortStatus.Draft),
                    "drafts",
                    _urlHelper.Action("Draft", "Cohort", new { source.ProviderId }),
                    CohortStatus.Draft.ToString()),
                CohortsInReview = new CohortCardLinkViewModel(
                    data.Cohorts.Count(x => x.GetStatus() == CohortStatus.Review),
                    "ready to review",
                    _urlHelper.Action("Review", "Cohort", new { source.ProviderId }),
                     CohortStatus.Review.ToString()),
                CohortsWithEmployer = new CohortCardLinkViewModel(
                    data.Cohorts.Count(x => x.GetStatus() == CohortStatus.WithEmployer),
                    "with employers",
                    _urlHelper.Action("WithEmployer", "Cohort", new { source.ProviderId }),
                    CohortStatus.WithEmployer.ToString()),
                CohortsWithTransferSender = new CohortCardLinkViewModel(
                    data.Cohorts.Count(x => x.GetStatus() == CohortStatus.WithTransferSender),
                    "with transfer sending employers",
                    _urlHelper.Action("WithTransferSender", "Cohort", new { source.ProviderId }),
                    CohortStatus.WithTransferSender.ToString()),
                IsAgreementSigned = data.providerAgreementStatus == ProviderAgreementStatus.Agreed 
            };
        }
    }
}
