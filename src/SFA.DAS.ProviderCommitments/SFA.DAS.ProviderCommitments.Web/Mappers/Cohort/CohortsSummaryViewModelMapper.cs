using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
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

        public CohortsSummaryViewModelMapper(ICommitmentsApiClient commitmentApiClient, IProviderRelationshipsApiClient providerRelationshipsApiClient, IUrlHelper urlHelper)
        {
            _commitmentsApiClient = commitmentApiClient;
            _providerRelationshipsApiClient = providerRelationshipsApiClient;
            _urlHelper = urlHelper;
        }

        public async Task<CohortsViewModel> Map(CohortsByProviderRequest source)
        {
            var getCohortsTask = _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });
            var hasRelationshipTask = _providerRelationshipsApiClient.HasRelationshipWithPermission( new HasRelationshipWithPermissionRequest {Ukprn = source.ProviderId, Operation = Operation.CreateCohort});

            await Task.WhenAll(getCohortsTask, hasRelationshipTask);

            var cohorts = (await getCohortsTask).Cohorts;

            return new CohortsViewModel
            {
                ProviderId = source.ProviderId,
                ShowDrafts = (await hasRelationshipTask),
                CohortsInDraft = new CohortCardLinkViewModel(
                    cohorts.Count(x => x.GetStatus() == CohortStatus.Draft),
                    "drafts",
                    _urlHelper.Action("Draft", "Cohort", new { source.ProviderId }),
                    CohortStatus.Draft.ToString()),
                CohortsInReview = new CohortCardLinkViewModel(
                    cohorts.Count(x => x.GetStatus() == CohortStatus.Review),
                    "ready to review",
                    _urlHelper.Action("Review", "Cohort", new { source.ProviderId }),
                     CohortStatus.Review.ToString()),
                CohortsWithEmployer = new CohortCardLinkViewModel(
                    cohorts.Count(x => x.GetStatus() == CohortStatus.WithEmployer),
                    "with employer",
                    _urlHelper.Action("WithEmployer", "Cohort", new { source.ProviderId }),
                    CohortStatus.WithEmployer.ToString()),
                CohortsWithTransferSender = new CohortCardLinkViewModel(
                    cohorts.Count(x => x.GetStatus() == CohortStatus.WithTransferSender),
                    "with transfer sending employers",
                    _urlHelper.Action("WithTransferSender", "Cohort", new { source.ProviderId }),
                    CohortStatus.WithTransferSender.ToString())
            };
        }
    }
}
