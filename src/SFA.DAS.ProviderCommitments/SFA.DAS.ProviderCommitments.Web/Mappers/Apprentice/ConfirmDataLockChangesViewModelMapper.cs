using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmDataLockChangesViewModelMapper : IMapper<ConfirmDataLockChangesRequest, ConfirmDataLockChangesViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ConfirmDataLockChangesViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public Task<ConfirmDataLockChangesViewModel> Map(ConfirmDataLockChangesRequest source)
        {
            //TODO : to get the employer name
            //var legalEntityTask =  _commitmentApiClient.GetAccountLegalEntity(newEmployerLegalEntityId);  

            throw new NotImplementedException();
        }
    }
}
