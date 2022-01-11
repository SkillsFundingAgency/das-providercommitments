using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class DraftRequestViewModelMapper : IMapper<CohortsByProviderRequest, DraftViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IProviderRelationshipsApiClient _providerRelationshipsApiClient;
        private readonly IUrlHelper _urlHelper;
        private readonly IPasAccountApiClient _pasAccountApiClient;
        private readonly IEncodingService _encodingService;

        public DraftRequestViewModelMapper(
            ICommitmentsApiClient commitmentApiClient,
            IProviderRelationshipsApiClient providerRelationshipsApiClient,
            IUrlHelper urlHelper,
            IPasAccountApiClient pasAccountApiClient,
            IEncodingService encodingSummary)
        {
            _commitmentsApiClient = commitmentApiClient;
            _providerRelationshipsApiClient = providerRelationshipsApiClient;
            _urlHelper = urlHelper;
            _pasAccountApiClient = pasAccountApiClient;
            _encodingService = encodingSummary;
        }

        public async Task<DraftViewModel> Map(CohortsByProviderRequest source)
        {
            async Task<(CohortSummary[] Cohorts, bool HasRelationship, ProviderAgreementStatus providerAgreementStatus)> GetData()
            {
                var getCohortsTask = _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });
                var hasRelationshipTask = _providerRelationshipsApiClient.HasRelationshipWithPermission(new HasRelationshipWithPermissionRequest { Ukprn = source.ProviderId, Operation = Operation.CreateCohort });
                var providerAgreement = _pasAccountApiClient.GetAgreement(source.ProviderId);

                await Task.WhenAll(getCohortsTask, hasRelationshipTask);
                return (getCohortsTask.Result.Cohorts, hasRelationshipTask.Result, providerAgreement.Result.Status);
            }

            var (cohorts, hasRelationship, providerAgreementStatus) = await GetData();
            var reviewViewModel = new DraftViewModel
            {
                ProviderId = source.ProviderId,
                SortField = source.SortField,
                ReverseSort = source.ReverseSort,
                ApprenticeshipRequestsHeaderViewModel = cohorts.GetCohortCardLinkViewModel(_urlHelper, source.ProviderId, CohortStatus.Draft, hasRelationship, providerAgreementStatus),
                Cohorts = cohorts
                    .Where(x => x.GetStatus() == CohortStatus.Draft)
                    .Select(y => new DraftCohortSummaryViewModel
                    {
                        CohortReference = _encodingService.Encode(y.CohortId, EncodingType.CohortReference),
                        EmployerName = y.LegalEntityName,
                        NumberOfApprentices = y.NumberOfDraftApprentices,
                        DateCreated = y.CreatedOn
                    })
                    .ApplyOrder(source.SortField, source.ReverseSort)
                    .ToList()
            };

            return reviewViewModel;
        }
    }

    internal static class DraftRequestViewModelMapperExtension
    {
        internal static IOrderedEnumerable<DraftCohortSummaryViewModel> ApplyOrder(this IEnumerable<DraftCohortSummaryViewModel> cohorts, string sortField, bool reverse)
        {
            switch (sortField)
            {
                case "Employer":
                {
                    if (reverse)
                        return cohorts
                            .OrderByDescending(c => c.EmployerName)
                            .ThenBy(c => c.DateCreated.Date)
                            .ThenBy(c => c.CohortReference);

                    return cohorts
                        .OrderBy(c => c.EmployerName)
                        .ThenBy(c => c.DateCreated.Date)
                        .ThenBy(c => c.CohortReference);
                }

                case "CohortReference":
                {
                    return reverse
                        ? cohorts.OrderByDescending(c => c.CohortReference)
                        : cohorts.OrderBy(c => c.CohortReference);
                }

                case "DateCreated":
                {
                    if (reverse)
                        return cohorts
                            .OrderByDescending(c => c.DateCreated.Date)
                            .ThenBy(c => c.EmployerName)
                            .ThenBy(c => c.CohortReference);

                    return cohorts
                        .OrderBy(c => c.DateCreated.Date)
                        .ThenBy(c => c.EmployerName)
                        .ThenBy(c => c.CohortReference);
                }
            }

            return cohorts
                .OrderBy(c => c.DateCreated.Date)
                .ThenBy(c => c.EmployerName)
                .ThenBy(c => c.CohortReference);
        }
    }
}
