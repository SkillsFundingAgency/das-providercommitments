using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ChangeEmployerViewModelMapper : IMapper<ChangeEmployerRequest, ChangeEmployerViewModel>
    {
        private readonly IProviderRelationshipsApiClient _providerRelationshipsApiClient;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ChangeEmployerViewModelMapper(IProviderRelationshipsApiClient providerRelationshipsApiClient, ICommitmentsApiClient commitmentsApiClient)
        {
            _providerRelationshipsApiClient = providerRelationshipsApiClient;
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<ChangeEmployerViewModel> Map(ChangeEmployerRequest source)
        {
            var apprenticeshipTask = _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);
            var legalEntities = await GetLegalEntitiesWithCreatePermission(source.ProviderId);
            var apprenticeShip = await apprenticeshipTask;
            var filteredLegalEntities = legalEntities.Where(x => x.AccountLegalEntityId != apprenticeShip.AccountLegalEntityId);
            return new ChangeEmployerViewModel
            {
                AccountProviderLegalEntities = (filteredLegalEntities.Select(x => new AccountProviderLegalEntityViewModel
                {
                    EmployerAccountLegalEntityName = x.AccountLegalEntityName,
                    EmployerAccountLegalEntityPublicHashedId = x.AccountLegalEntityPublicHashedId,
                    EmployerAccountName = x.AccountName,
                    EmployerAccountPublicHashedId = x.AccountPublicHashedId,
                })).ToList()
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
    }

}
