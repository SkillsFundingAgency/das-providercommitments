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
            var result = await _providerRelationshipsApiClient.GetAccountProviderLegalEntitiesWithPermission(
                new GetAccountProviderLegalEntitiesWithPermissionRequest
                {
                    Ukprn = source.ProviderId,
                    Operation = Operation.CreateCohort
                });

            var viewModelList = new List<AccountProviderLegalEntityViewModel>();
            foreach (var apiEntity in result.AccountProviderLegalEntities)
            {
                viewModelList.Add(new AccountProviderLegalEntityViewModel
                {
                    EmployerAccountLegalEntityName = apiEntity.AccountLegalEntityName,
                    EmployerAccountLegalEntityPublicHashedId = apiEntity.AccountLegalEntityPublicHashedId,
                    EmployerAccountName = apiEntity.AccountName,
                    EmployerAccountPublicHashedId = apiEntity.AccountPublicHashedId,
                    //SelectEmployerUrl = _linkGenerator.ProviderApprenticeshipServiceLink()
                });
            }

            return new SelectEmployerViewModel
            {
                AccountProviderLegalEntities = viewModelList,
                BackLink = _linkGenerator.ProviderApprenticeshipServiceLink("account")
            };
        }

    }
}
