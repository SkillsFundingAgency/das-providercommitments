﻿using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectEmployerViewModelMapper : IMapper<SelectEmployerRequest, SelectEmployerViewModel>
    {
        private readonly IProviderRelationshipsApiClient _providerRelationshipsApiClient;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public SelectEmployerViewModelMapper(IProviderRelationshipsApiClient providerRelationshipsApiClient, ICommitmentsApiClient commitmentsApiClient)
        {
            _providerRelationshipsApiClient = providerRelationshipsApiClient;
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<SelectEmployerViewModel> Map(SelectEmployerRequest source)
        {
            var apprenticeshipTask = _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);
            var legalEntities = await GetLegalEntitiesWithCreatePermission(source.ProviderId);
            var apprenticeship = await apprenticeshipTask;

            var accountProviderLegalEntities = legalEntities
                .Where(x => x.AccountLegalEntityId != apprenticeship.AccountLegalEntityId)
                .Select(x => new AccountProviderLegalEntityViewModel
                    {
                        EmployerAccountLegalEntityName = x.AccountLegalEntityName,
                        EmployerAccountLegalEntityPublicHashedId = x.AccountLegalEntityPublicHashedId,
                        EmployerAccountName = x.AccountName,
                        EmployerAccountPublicHashedId = x.AccountPublicHashedId
                    })
                .ToList();

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
                LegalEntityName = apprenticeship.EmployerName,
                AccountProviderLegalEntities = accountProviderLegalEntities,
                SelectEmployerFilterModel = filterModel,
            };
        }

        private async Task<IEnumerable<AccountProviderLegalEntityDto>> GetLegalEntitiesWithCreatePermission(long providerId)
        {
            var result = await _providerRelationshipsApiClient.GetAccountProviderLegalEntitiesWithPermission(
                new GetAccountProviderLegalEntitiesWithPermissionRequest
                {
                    Ukprn = providerId,
                    Operation = Operation.CreateCohort
                });

            if (result?.AccountProviderLegalEntities == null)
            {
                return new List<AccountProviderLegalEntityDto>();
            }

            return result.AccountProviderLegalEntities;
        }

        private static List<AccountProviderLegalEntityViewModel> ApplySort(List<AccountProviderLegalEntityViewModel> accountProviderLegalEntities, SelectEmployerFilterModel filterModel)
        {
            if (!string.IsNullOrWhiteSpace(filterModel.CurrentlySortedByField))
            {
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
            }

            return accountProviderLegalEntities;
        }

        private static List<AccountProviderLegalEntityViewModel> ApplySearch(SelectEmployerRequest source, List<AccountProviderLegalEntityViewModel> accountProviderLegalEntities, SelectEmployerFilterModel filterModel)
        {
            if (!string.IsNullOrWhiteSpace(source.SearchTerm))
            {
                if (!string.IsNullOrWhiteSpace(filterModel.SearchAccountName))
                {
                    accountProviderLegalEntities = accountProviderLegalEntities.Where(x => x.EmployerAccountLegalEntityName.ToLower().Contains(filterModel.SearchEmployerName)
                    && x.EmployerAccountName.ToLower().Contains(filterModel.SearchAccountName)).ToList();
                }
                else
                {
                    accountProviderLegalEntities = accountProviderLegalEntities
                        .Where(x => x.EmployerAccountName.ToLower().Contains(filterModel.SearchEmployerName) ||
                            x.EmployerAccountLegalEntityName.ToLower().Contains(filterModel.SearchEmployerName)).ToList();
                }
            }

            return accountProviderLegalEntities;
        }
    }
}
