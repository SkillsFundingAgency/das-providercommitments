using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class ConfirmEmployerViewModelToCreateEmptyCohortRequestMapper : IMapper<ConfirmEmployerViewModel, CreateEmptyCohortRequest>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ConfirmEmployerViewModelToCreateEmptyCohortRequestMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<CreateEmptyCohortRequest> Map(ConfirmEmployerViewModel source)
        {
            var accountLegalEntity = await _commitmentsApiClient.GetAccountLegalEntity(source.AccountLegalEntityId);
            return new CreateEmptyCohortRequest
            {
                AccountId = accountLegalEntity.AccountId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                ProviderId = source.ProviderId,
            };
        }
    }
}
