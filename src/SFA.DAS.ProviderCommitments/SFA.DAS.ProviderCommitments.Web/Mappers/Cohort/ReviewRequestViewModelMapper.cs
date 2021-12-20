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
    public class ReviewRequestViewModelMapper : IMapper<CohortsByProviderRequest, ReviewViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IProviderRelationshipsApiClient _providerRelationshipsApiClient;
        private readonly IUrlHelper _urlHelper;
        private readonly IPasAccountApiClient _pasAccountApiClient;
        private readonly IEncodingService _encodingService;

        public ReviewRequestViewModelMapper(
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

        public async Task<ReviewViewModel> Map(CohortsByProviderRequest source)
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

            var reviewViewModel = new ReviewViewModel
            {
                ProviderId = source.ProviderId,
                SortField = source.SortField,
                ReverseSort = source.ReverseSort,
                ApprenticeshipRequestsHeaderViewModel = cohorts.GetCohortCardLinkViewModel(_urlHelper, source.ProviderId, CohortStatus.Review, hasRelationship, providerAgreementStatus),
                Cohorts = cohorts
                    .Where(x => x.GetStatus() == CohortStatus.Review)
                    .Select(y => new ReviewCohortSummaryViewModel
                    {
                        CohortReference = _encodingService.Encode(y.CohortId, EncodingType.CohortReference),
                        EmployerName = y.LegalEntityName,
                        NumberOfApprentices = y.NumberOfDraftApprentices,
                        LastMessage = GetMessage(y.LatestMessageFromEmployer),
                        DateReceived = y.LatestMessageFromEmployer?.SentOn ?? y.CreatedOn
                    })
                    .ApplyOrder(source.SortField, source.ReverseSort)
                    .ToList()
            };

            return reviewViewModel;
        }

        private string GetMessage(Message latestMessageFromProvider)
        {
            return !string.IsNullOrWhiteSpace(latestMessageFromProvider?.Text) 
                ? latestMessageFromProvider.Text 
                : "No message added";
        }
    }

    internal static class ReviewRequestViewModelMapperExtension
    {
        internal static IOrderedEnumerable<ReviewCohortSummaryViewModel> ApplyOrder(this IEnumerable<ReviewCohortSummaryViewModel> cohorts, string sortField, bool reverse)
        {
            switch (sortField)
            {
                case "Employer":
                {
                    if (reverse)
                        return cohorts
                            .OrderByDescending(c => c.EmployerName)
                            .ThenBy(c => c.DateReceived.Date)
                            .ThenBy(c => c.CohortReference);

                    return cohorts
                        .OrderBy(c => c.EmployerName)
                        .ThenBy(c => c.DateReceived.Date)
                        .ThenBy(c => c.CohortReference);
                }

                case "CohortReference":
                {
                    return reverse 
                        ? cohorts.OrderByDescending(c => c.CohortReference) 
                        : cohorts.OrderBy(c => c.CohortReference);
                }

                case "DateReceived":
                {
                    if (reverse)
                        return cohorts
                            .OrderByDescending(c => c.DateReceived.Date)
                            .ThenBy(c => c.EmployerName)
                            .ThenBy(c => c.CohortReference);

                    return cohorts
                        .OrderBy(c => c.DateReceived.Date)
                        .ThenBy(c => c.EmployerName)
                        .ThenBy(c => c.CohortReference);
                }
            }

            return cohorts
                .OrderBy(c => c.DateReceived.Date)
                .ThenBy(c => c.EmployerName)
                .ThenBy(c => c.CohortReference);
        }
    }
}
