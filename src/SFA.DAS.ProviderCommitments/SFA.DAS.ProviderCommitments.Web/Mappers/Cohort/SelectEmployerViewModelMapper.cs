using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectEmployerViewModelMapper : IMapper<SelectEmployerRequest, SelectEmployerViewModel>
    {
        private readonly IApprovalsOuterApiClient _approvalsOuterApiClient;

        public SelectEmployerViewModelMapper(IApprovalsOuterApiClient approvalsOuterApiClient) => _approvalsOuterApiClient = approvalsOuterApiClient;

        public async Task<SelectEmployerViewModel> Map(SelectEmployerRequest source)
        {
            var accountProviderLegalEntities = await GetAccountProviderLegalEntities(source);
            accountProviderLegalEntities = await GetAccountsLevyStatus(accountProviderLegalEntities);
            var filterModel = new SelectEmployerFilterModel
            {
                SearchTerm = source.SearchTerm,
                ReverseSort = source.ReverseSort,
                CurrentlySortedByField = source.SortField,
                UseLearnerData = source.UseLearnerData,
                Employers = accountProviderLegalEntities.SelectMany(x => (new List<string> { x.EmployerAccountLegalEntityName, x.EmployerAccountName }).Distinct()).ToList()
            };

            accountProviderLegalEntities = ApplySearch(source, accountProviderLegalEntities, filterModel);

            accountProviderLegalEntities = ApplySort(accountProviderLegalEntities, filterModel);

            return new SelectEmployerViewModel
            {
                AccountProviderLegalEntities = accountProviderLegalEntities,
                ProviderId = source.ProviderId,
                SelectEmployerFilterModel = filterModel,
                UseLearnerData = source.UseLearnerData
            };
        }
        private async Task<List<AccountProviderLegalEntityViewModel>> GetAccountsLevyStatus(List<AccountProviderLegalEntityViewModel> source)
        {
            foreach (var providerAccount in source)
            {
                var account = await _approvalsOuterApiClient.GetAccount(providerAccount.AccountHashedId);
                if (Enum.TryParse<ApprenticeshipEmployerType>(account?.ApprenticeshipEmployerType, out var levyStatus))
                {
                    providerAccount.LevyStatus = levyStatus;
                }
                else
                {
                    providerAccount.LevyStatus = ApprenticeshipEmployerType.NonLevy;
                }
            }
            return source;
        }

        private async Task<List<AccountProviderLegalEntityViewModel>> GetAccountProviderLegalEntities(SelectEmployerRequest source)
        {
            var result = (await GetLegalEntitiesWithCreatePermission(source.ProviderId));

            if (result.AccountProviderLegalEntities != null && result.AccountProviderLegalEntities.Any())
            {
                return result.AccountProviderLegalEntities.Select(x => new AccountProviderLegalEntityViewModel
                {
                    EmployerAccountLegalEntityName = x.AccountLegalEntityName,
                    EmployerAccountLegalEntityPublicHashedId = x.AccountLegalEntityPublicHashedId,
                    EmployerAccountName = x.AccountName,
                    EmployerAccountPublicHashedId = x.AccountPublicHashedId,
                    AccountHashedId = x.AccountHashedId
                }).ToList();
            }

            return new List<AccountProviderLegalEntityViewModel>();
        }

        private static List<AccountProviderLegalEntityViewModel> ApplySort(List<AccountProviderLegalEntityViewModel> accountProviderLegalEntities, SelectEmployerFilterModel filterModel)
        {
            if (string.IsNullOrWhiteSpace(filterModel.CurrentlySortedByField))
            {
                return accountProviderLegalEntities;
            }

            if (filterModel.CurrentlySortedByField == SelectEmployerFilterModel.EmployerAccountLegalEntityNameConst)
            {
                accountProviderLegalEntities = (filterModel.ReverseSort
                    ? accountProviderLegalEntities.OrderByDescending(x => x.EmployerAccountLegalEntityName)
                        .ThenBy(x => x.EmployerAccountName)
                        .ThenBy(x => x.EmployerAccountLegalEntityPublicHashedId)

                    : accountProviderLegalEntities.OrderBy(x => x.EmployerAccountLegalEntityName)
                        .ThenBy(x => x.EmployerAccountName)
                        .ThenBy(x => x.EmployerAccountLegalEntityPublicHashedId)).ToList();
            }
            else
            {
                accountProviderLegalEntities = (filterModel.ReverseSort
                    ? accountProviderLegalEntities.OrderByDescending(x => x.EmployerAccountName)
                        .ThenBy(x => x.EmployerAccountLegalEntityName)
                        .ThenBy(x => x.EmployerAccountLegalEntityPublicHashedId)

                    : accountProviderLegalEntities.OrderBy(x => x.EmployerAccountName)
                        .ThenBy(x => x.EmployerAccountLegalEntityName)
                        .ThenBy(x => x.EmployerAccountLegalEntityPublicHashedId)).ToList();
            }

            return accountProviderLegalEntities;
        }

        private static List<AccountProviderLegalEntityViewModel> ApplySearch(SelectEmployerRequest source, List<AccountProviderLegalEntityViewModel> accountProviderLegalEntities, SelectEmployerFilterModel filterModel)
        {
            if (string.IsNullOrWhiteSpace(source.SearchTerm))
            {
                return accountProviderLegalEntities;
            }

            if (!string.IsNullOrWhiteSpace(filterModel.SearchAccountName))
            {
                accountProviderLegalEntities = accountProviderLegalEntities.Where(x => 
                    x.EmployerAccountLegalEntityName.Contains(filterModel.SearchEmployerName, StringComparison.CurrentCultureIgnoreCase)
                    && x.EmployerAccountName.Contains(filterModel.SearchAccountName, StringComparison.CurrentCultureIgnoreCase)
                    ).ToList();
            }
            else
            {
                accountProviderLegalEntities = accountProviderLegalEntities.Where(x => 
                    x.EmployerAccountName.Contains(filterModel.SearchEmployerName, StringComparison.CurrentCultureIgnoreCase)
                    ||x.EmployerAccountLegalEntityName.Contains(filterModel.SearchEmployerName, StringComparison.CurrentCultureIgnoreCase)
                    || x.EmployerAccountLegalEntityPublicHashedId.Contains(filterModel.SearchEmployerName, StringComparison.CurrentCultureIgnoreCase)
                    ).ToList();
            }

            return accountProviderLegalEntities;
        }

        private async Task<GetProviderAccountLegalEntitiesResponse> GetLegalEntitiesWithCreatePermission(long providerId)
        {
            var result = await _approvalsOuterApiClient.GetProviderAccountLegalEntities((int)providerId);

            return result ?? new GetProviderAccountLegalEntitiesResponse();
        }
    }
}
