using FluentAssertions;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
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

            var filterModel = new SelectEmployerFilterModel
            {
                SearchTerm = source.SearchTerm,
                ReverseSort = source.ReverseSort,
                CurrentlySortedByField = source.SortField,
                Employers = accountProviderLegalEntities.SelectMany(x => (new List<string> { x.EmployerAccountLegalEntityName, x.EmployerAccountName }).Distinct()).ToList()
            };

            accountProviderLegalEntities = ApplySearch(source, accountProviderLegalEntities, filterModel);

            accountProviderLegalEntities = ApplySort(accountProviderLegalEntities, filterModel);

            return new SelectEmployerViewModel
            {
                AccountProviderLegalEntities = accountProviderLegalEntities,
                ProviderId = source.ProviderId,
                SelectEmployerFilterModel = filterModel
            };
        }

        private async Task<List<AccountProviderLegalEntityViewModel>> GetAccountProviderLegalEntities(SelectEmployerRequest source)
        {
            return (await GetLegalEntitiesWithCreatePermission(source.ProviderId)).AccountProviderLegalEntities.Select(x => new AccountProviderLegalEntityViewModel
            {
                EmployerAccountLegalEntityName = x.AccountLegalEntityName,
                EmployerAccountLegalEntityPublicHashedId = x.AccountLegalEntityPublicHashedId,
                EmployerAccountName = x.AccountName,
                EmployerAccountPublicHashedId = x.AccountPublicHashedId,
            }).ToList();
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
                accountProviderLegalEntities = accountProviderLegalEntities.Where(x => x.EmployerAccountLegalEntityName.ToLower().Contains(filterModel.SearchEmployerName)
                    && x.EmployerAccountName.ToLower().Contains(filterModel.SearchAccountName)).ToList();
            }
            else
            {
                accountProviderLegalEntities = accountProviderLegalEntities.Where(x => x.EmployerAccountName.ToLower().Contains(filterModel.SearchEmployerName)
                    ||
                    x.EmployerAccountLegalEntityName.ToLower().Contains(filterModel.SearchEmployerName)).ToList();
            }

            return accountProviderLegalEntities;
        }

        private async Task<GetProviderAccountLegalEntitiesResponse> GetLegalEntitiesWithCreatePermission(long providerId)
        {
            var result = await _approvalsOuterApiClient.GetProviderAccountLegalEntities((int)providerId, Operation.CreateCohort.ToString(), "");

            return result ?? new GetProviderAccountLegalEntitiesResponse();
        }
    }
}
