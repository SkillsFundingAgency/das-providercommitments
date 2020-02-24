using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Requests.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

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
            var apprenticeShip = await apprenticeshipTask;
            var filteredLegalEntities = legalEntities.Where(x => x.AccountLegalEntityId != apprenticeShip.AccountLegalEntityId);
            return new SelectEmployerViewModel
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
