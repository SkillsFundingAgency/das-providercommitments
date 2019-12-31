using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class SelectEmployerViewModelMapper : IMapper<SelectEmployerRequest, SelectEmployerViewModel>
    {
        private readonly IProviderRelationshipsApiClient _providerRelationshipsApiClient;
        private readonly ILinkGenerator _linkGenerator;

        public SelectEmployerViewModelMapper(IProviderRelationshipsApiClient providerRelationshipsApiClient, ILinkGenerator linkGenerator)
        {
            _providerRelationshipsApiClient = providerRelationshipsApiClient;
            _linkGenerator = linkGenerator;
        }

        public async Task<SelectEmployerViewModel> Map(SelectEmployerRequest source)
        {
            return new SelectEmployerViewModel
            {
                AccountProviderLegalEntities = (await GetLegalEntitiesWithCreatePermission(source.ProviderId)).Select(x=>new AccountProviderLegalEntityViewModel
                {
                    EmployerAccountLegalEntityName = x.AccountLegalEntityName,
                    EmployerAccountLegalEntityPublicHashedId = x.AccountLegalEntityPublicHashedId,
                    EmployerAccountName = x.AccountName,
                    EmployerAccountPublicHashedId = x.AccountPublicHashedId,
                }).ToList(),
                BackLink = _linkGenerator.ProviderApprenticeshipServiceLink("account"),
                ProviderId = source.ProviderId
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
