using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using WebApp= SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using API=SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateEmptyCohort;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class CreateEmptyCohortRequestToApiCreateEmptyCohortRequestMapper : IMapper<CreateEmptyCohortRequest, API.CreateEmptyCohortRequest>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public CreateEmptyCohortRequestToApiCreateEmptyCohortRequestMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<API.CreateEmptyCohortRequest> Map(CreateEmptyCohortRequest source)
        {
            var accountLegalEntity = await _commitmentsApiClient.GetLegalEntity(source.AccountLegalEntityId);
            return new API.CreateEmptyCohortRequest
            {
                AccountId = accountLegalEntity.AccountId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                ProviderId = source.ProviderId
            };
        }
    }
}
