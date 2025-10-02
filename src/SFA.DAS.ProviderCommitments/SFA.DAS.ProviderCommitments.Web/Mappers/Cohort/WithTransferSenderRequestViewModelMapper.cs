using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderCommitments.Enums;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class WithTransferSenderRequestViewModelMapper : IMapper<CohortsByProviderRequest, WithTransferSenderViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IApprovalsOuterApiClient _approvalsOuterApiClient;
        private readonly IUrlHelper _urlHelper;
        private readonly IPasAccountApiClient _pasAccountApiClient;
        private readonly IEncodingService _encodingService;
        private readonly IOuterApiClient _outerApiClient;

        public WithTransferSenderRequestViewModelMapper(
            ICommitmentsApiClient commitmentApiClient,
            IApprovalsOuterApiClient approvalsOuterApiClient,
            IUrlHelper urlHelper,
            IPasAccountApiClient pasAccountApiClient,
            IEncodingService encodingSummary,
            IOuterApiClient outerApiClient)
        {
            _commitmentsApiClient = commitmentApiClient;
            _approvalsOuterApiClient = approvalsOuterApiClient;
            _urlHelper = urlHelper;
            _pasAccountApiClient = pasAccountApiClient;
            _encodingService = encodingSummary;
            _outerApiClient = outerApiClient;
        }

        public async Task<WithTransferSenderViewModel> Map(CohortsByProviderRequest source)
        {
            async Task<(CohortSummary[] Cohorts, bool HasRelationship, ProviderStatusType providerStatus)>
                GetData()
            {
                var getCohortsTask = _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });
                var hasRelationshipTask = _approvalsOuterApiClient.GetHasRelationshipWithPermission(source.ProviderId);
                var providerStatusTask = _outerApiClient.Get<GetProviderDetailsResponse>(new GetProviderDetailsRequest(source.ProviderId));

                await Task.WhenAll(getCohortsTask, hasRelationshipTask, providerStatusTask);
                return (getCohortsTask.Result.Cohorts, hasRelationshipTask.Result.HasPermission, providerStatusTask.Result.ProviderStatus);
            }

            var (cohorts, hasRelationship, providerAgreementStatus) = await GetData();
            var reviewViewModel = new WithTransferSenderViewModel
            {
                ProviderId = source.ProviderId,
                SortField = source.SortField,
                ReverseSort = source.ReverseSort,
                ApprenticeshipRequestsHeaderViewModel = cohorts.GetCohortCardLinkViewModel(_urlHelper, source.ProviderId, CohortStatus.WithTransferSender, hasRelationship, providerAgreementStatus),
                Cohorts = cohorts
                    .Where(x => x.GetStatus() == CohortStatus.WithTransferSender)
                    .Select(y => new WithTransferSenderCohortSummaryViewModel
                    {
                        CohortReference = _encodingService.Encode(y.CohortId, EncodingType.CohortReference),
                        EmployerName = y.LegalEntityName,
                        NumberOfApprentices = y.NumberOfDraftApprentices,
                        DateSentToEmployer = GetOrderByDate(y)
                    })
                    .ApplyOrder(source.SortField, source.ReverseSort)
                    .ToList()
            };

            return reviewViewModel;
        }

        private static DateTime GetOrderByDate(CohortSummary s)
        {
            return new[] { s.LatestMessageFromEmployer?.SentOn, s.LatestMessageFromProvider?.SentOn, s.CreatedOn }.Max().Value;
        }
    }

    internal static class WithTransferSenderRequestViewModelMapperExtension
    {
        internal static IOrderedEnumerable<WithTransferSenderCohortSummaryViewModel> ApplyOrder(this IEnumerable<WithTransferSenderCohortSummaryViewModel> cohorts, string sortField, bool reverse)
        {
            switch (sortField)
            {
                case "Employer":
                    {
                        if (reverse)
                            return cohorts
                                .OrderByDescending(c => c.EmployerName)
                                .ThenBy(c => c.DateSentToEmployer.Date)
                                .ThenBy(c => c.CohortReference);

                        return cohorts
                            .OrderBy(c => c.EmployerName)
                            .ThenBy(c => c.DateSentToEmployer.Date)
                            .ThenBy(c => c.CohortReference);
                    }

                case "CohortReference":
                    {
                        return reverse
                            ? cohorts.OrderByDescending(c => c.CohortReference)
                            : cohorts.OrderBy(c => c.CohortReference);
                    }

                case "DateSentToEmployer":
                    {
                        if (reverse)
                            return cohorts
                                .OrderByDescending(c => c.DateSentToEmployer.Date)
                                .ThenBy(c => c.EmployerName)
                                .ThenBy(c => c.CohortReference);

                        return cohorts
                            .OrderBy(c => c.DateSentToEmployer.Date)
                            .ThenBy(c => c.EmployerName)
                            .ThenBy(c => c.CohortReference);
                    }
            }

            return cohorts
                .OrderBy(c => c.DateSentToEmployer.Date)
                .ThenBy(c => c.EmployerName)
                .ThenBy(c => c.CohortReference);
        }
    }
}
